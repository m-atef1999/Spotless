using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spotless.Application.Dtos.Authentication;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.ValueObjects;
using Spotless.Infrastructure.Identity;

namespace Spotless.Infrastructure.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork, IEmailService emailService, ISmsService smsService) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IEmailService _emailService = emailService;
        private readonly ISmsService _smsService = smsService;

        public async Task<AuthResult> RegisterAsync(RegisterRequest request, string role)
        {
            var user = new ApplicationUser { UserName = request.Email, Email = request.Email, IsActive = true };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new Exception("Identity creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, role);

            if (role == "Customer")
            {
                var address = new Address(
                    request.Street,
                    request.City,
                    request.Country,
                    request.ZipCode
                );
                var customer = new Customer(
                    adminId: null,
                    name: request.Name,
                    phone: request.Phone,
                    email: request.Email,
                    address: address,
                    type: request.Type);

                await _unitOfWork.Customers.AddAsync(customer);
                user.CustomerId = customer.Id;
                await _userManager.UpdateAsync(user);
            }

            await _unitOfWork.CommitAsync();



            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();


            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);


            var authResult = _jwtTokenGenerator.GenerateAuthResult(user, role, refreshToken);
            return authResult;

        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Unknown";


            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();


            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);


            var authResult = _jwtTokenGenerator.GenerateAuthResult(user, role, refreshToken);
            return authResult;

        }



        public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);


            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {

                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            if (user == null)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Unknown";


            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();


            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);


            var authResult = _jwtTokenGenerator.GenerateAuthResult(user, role, newRefreshToken);

            return authResult;
        }
        public async Task<bool> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                currentPassword,
                newPassword
            );

            return result.Succeeded;
        }

        public async Task<bool> SendVerificationEmailAsync(Guid userId, string clientBaseUrl)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || user.Email == null || user.EmailConfirmed)
            {
                return false;
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = System.Text.Encodings.Web.UrlEncoder.Default.Encode(token);
            var callbackUrl = $"{clientBaseUrl}/confirm-email?userId={user.Id}&token={encodedToken}";

            var emailBody = $"Please confirm your account by clicking here: <a href='{callbackUrl}'>link</a>";

            await _emailService.SendEmailAsync(
                user.Email,
                "Confirm Your Spotless Account",
                emailBody
            );

            return true;
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded;
        }

        public async Task<bool> ForgotPasswordAsync(string email, string clientBaseUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return true;
            }


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);


            var encodedToken = System.Text.Encodings.Web.UrlEncoder.Default.Encode(token);
            var callbackUrl = $"{clientBaseUrl}/reset-password?userId={user.Id}&token={encodedToken}";


            var emailBody = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>";

            await _emailService.SendEmailAsync(
                email,
                "Reset Password",
                emailBody
            );

            return true;
        }
        public async Task<bool> SendPhoneVerificationOtpAsync(string phoneNumber)
        {

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {

                return true;
            }


            if (user.PhoneNumberConfirmed)
            {

                return true;
            }

            return await _smsService.SendOtpAsync(phoneNumber);
        }
        public async Task<bool> VerifyPhoneOtpAsync(string phoneNumber, string code)
        {

            var isCodeValid = await _smsService.VerifyOtpAsync(phoneNumber, code);

            if (!isCodeValid)
            {
                return false;
            }


            var user = await _userManager.Users
                                         .SingleOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user == null)
            {

                return false;
            }


            user.PhoneNumberConfirmed = true;
            var updateResult = await _userManager.UpdateAsync(user);

            return updateResult.Succeeded;
        }

        public async Task<Guid> CreateUserAsync(string email, string password, string role)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception("Identity creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, role);

            return user.Id;
        }

        public async Task<AuthResult> ExternalLoginAsync(string provider, string idToken)
        {
            if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(idToken))
                throw new ArgumentException("Provider and idToken must be provided.");

            if (!provider.Equals("Google", StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException($"Provider '{provider}' is not supported.");

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            }
            catch (InvalidJwtException ex)
            {
                throw new UnauthorizedAccessException("Invalid Google token.", ex);
            }

            var email = payload.Email;
            if (string.IsNullOrEmpty(email))
                throw new Exception("Google token does not contain an email.");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // create a local user for this external account
                var tempPassword = Guid.NewGuid().ToString("N") + "aA!1";
                var newUserId = await CreateUserAsync(email, tempPassword, "Customer");
                user = await _userManager.FindByIdAsync(newUserId.ToString());
            }

            if (user == null)
                throw new UnauthorizedAccessException("Unable to create or find user for external login.");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Customer";

            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var authResult = _jwtTokenGenerator.GenerateAuthResult(user, role, refreshToken);

            return authResult;
        }
    }

}
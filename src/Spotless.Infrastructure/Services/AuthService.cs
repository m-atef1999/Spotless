using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spotless.Application.Dtos.Authentication;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.ValueObjects;
using Spotless.Domain.Enums;
using Spotless.Infrastructure.Identity;

using System.Net.Http;
using System.Text.Json;

namespace Spotless.Infrastructure.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork, IEmailService emailService, ISmsService smsService, IHttpClientFactory httpClientFactory) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IEmailService _emailService = emailService;
        private readonly ISmsService _smsService = smsService;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<AuthResult> RegisterAsync(RegisterRequest request, string role)
        {
            var address = new Address(
                request.Street,
                request.City,
                request.Country,
                request.ZipCode
            );

            var user = new ApplicationUser 
            { 
                UserName = request.Email, 
                Email = request.Email, 
                IsActive = true,
                Name = request.Name,
                Address = address
            };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                throw new ArgumentException("Failed to assign role: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            if (role == "Customer")
            {
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

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Unknown";


            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();


            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);


            var authResult = _jwtTokenGenerator.GenerateAuthResult(user, role, newRefreshToken);

            return authResult;
        }

        public async Task<AuthResult> ExternalLoginAsync(string provider, string idToken)
        {
            if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(idToken))
                throw new ArgumentException("Provider and idToken must be provided.");

            if (!provider.Equals("Google", StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException($"Provider '{provider}' is not supported.");

            string? email = null;
            string? providerKey = null;

            // 1. Try validating as ID Token (JWT)
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                email = payload.Email;
                providerKey = payload.Subject; // This is the unique Google User ID
            }
            catch (InvalidJwtException)
            {
                // 2. If invalid JWT, assume it's an Access Token and try UserInfo endpoint
                try
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", idToken);

                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(content);
                        if (doc.RootElement.TryGetProperty("email", out var emailElement))
                        {
                            email = emailElement.GetString();
                        }
                        if (doc.RootElement.TryGetProperty("sub", out var subElement))
                        {
                            providerKey = subElement.GetString();
                        }
                    }
                }
                catch
                {
                    // Ignore and fall through
                }
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(providerKey))
                throw new UnauthorizedAccessException("Invalid Google token or unable to retrieve email/user ID.");

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // create a local user for this external account
                var tempPassword = Guid.NewGuid().ToString("N") + "aA!1";
                var newUserId = await CreateUserAsync(email, tempPassword, "Customer");
                user = await _userManager.FindByIdAsync(newUserId.ToString());

                // Link the external login immediately for the new user
            }
            else
            {
                // Check if user exists but is not linked to Google
                var logins = await _userManager.GetLoginsAsync(user);
                var isLinked = logins.Any(l => l.LoginProvider == provider && l.ProviderKey == providerKey);

                if (!isLinked)
                {
                    // Automatically link the account
                    var loginInfo = new UserLoginInfo(provider, providerKey, provider);
                    var linkResult = await _userManager.AddLoginAsync(user, loginInfo);
                    
                    if (!linkResult.Succeeded)
                    {
                        // If linking fails, we might want to log it, but for now we can proceed 
                        // or throw if it's critical. Usually it fails if the login is already used by another user.
                        // But we already checked by email, so it's the same user.
                        // Just in case, let's throw to be safe.
                         throw new Exception("Failed to link Google account: " + string.Join(", ", linkResult.Errors.Select(e => e.Description)));
                    }
                }
            }

            if (user == null)
                throw new UnauthorizedAccessException("Unable to create or find user for external login.");

            // Ensure Customer record exists
            if (user.CustomerId == null)
            {
                // Try to find existing customer by email
                var existingCustomer = await _unitOfWork.Customers.GetByEmailAsync(email);
                
                if (existingCustomer != null)
                {
                    user.CustomerId = existingCustomer.Id;
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    // Create new customer
                    string name = email.Split('@')[0];
                    
                    var customer = new Customer(
                        adminId: null,
                        name: name,
                        phone: null, // Phone is optional
                        email: email,
                        address: new Address("Unknown", "Unknown", "Unknown", "00000"), // Use placeholders to pass validation
                        type: CustomerType.Individual); // Default type

                    await _unitOfWork.Customers.AddAsync(customer);
                    await _unitOfWork.CommitAsync();

                    user.CustomerId = customer.Id;
                    await _userManager.UpdateAsync(user);
                }
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Customer";

            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var authResult = _jwtTokenGenerator.GenerateAuthResult(user, role, refreshToken);

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

        public async Task UpdateUserProfileAsync(Guid userId, string name, Domain.ValueObjects.Address address, string? phoneNumber)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.Name = name;
                user.Address = address;
                if (phoneNumber != null) user.PhoneNumber = phoneNumber;

                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<string?> GetUserIdByCustomerIdAsync(Guid customerId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.CustomerId == customerId);
            return user?.Id.ToString();
        }

        public async Task<List<string>> GetAdminUserIdsAsync()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return admins.Select(u => u.Id.ToString()).ToList();
        }

        public async Task AddRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        public async Task RemoveRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }
        }

        public async Task LinkAdminAsync(Guid userId, Guid adminId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.AdminId = adminId;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<string?> GetUserIdByDriverIdAsync(Guid driverId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.DriverId == driverId);
            return user?.Id.ToString();
        }
    }

}

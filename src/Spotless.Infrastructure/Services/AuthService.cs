using Microsoft.AspNetCore.Identity;
using Spotless.Application.Dtos.Authentication;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;
using Spotless.Infrastructure.Identity;


namespace Spotless.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _unitOfWork = unitOfWork;
        }

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
                var customer = new Customer(
                    adminId: null,
                    name: request.Name,
                    phone: request.Phone,
                    email: request.Email,
                    address: new Address("Default", "N/A", "N/A", "N/A"), // Placeholder Address VO
                    type: CustomerType.Individual);

                await _unitOfWork.Customers.AddAsync(customer);


                user.CustomerId = customer.Id;
                await _userManager.UpdateAsync(user);
            }


            await _unitOfWork.CommitAsync();


            var token = _jwtTokenGenerator.GenerateToken(user, role);

            return new AuthResult(user.Id, user.Email!, token, DateTime.UtcNow.AddMinutes(60));
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

            var token = _jwtTokenGenerator.GenerateToken(user, role);

            return new AuthResult(user.Id, user.Email!, token, DateTime.UtcNow.AddMinutes(60));
        }
    }
}

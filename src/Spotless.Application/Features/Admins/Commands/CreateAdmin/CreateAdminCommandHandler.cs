using MediatR;
using Spotless.Application.Dtos.Authentication;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Admins.Commands.CreateAdmin
{
    public class CreateAdminCommandHandler(
        IAuthService authService,
        IUnitOfWork unitOfWork) : IRequestHandler<CreateAdminCommand, AuthResult>
    {
        private readonly IAuthService _authService = authService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AuthResult> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
        {
            if (await _authService.UserExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email already in use");
            }

            var admin = new Admin(
                request.Name,
                request.Email,
                Enum.TryParse<AdminRole>(request.Role, true, out var role) ? role : AdminRole.SuperAdmin
            );

            await _unitOfWork.Admins.AddAsync(admin);
            await _unitOfWork.CommitAsync();

            try
            {
                var userId = await _authService.CreateUserAsync(request.Email, request.Password, UserRole.Admin.ToString());
                await _authService.LinkAdminAsync(userId, admin.Id);
                
                // Return a successful result without tokens (admin creation doesn't require login)
                return new AuthResult(
                    userId,
                    request.Email,
                    string.Empty,
                    DateTime.UtcNow,
                    string.Empty,
                    DateTime.UtcNow,
                    UserRole.Admin.ToString()
                );
            }
            catch (Exception ex)
            {
                // Note: Admin entity is already committed, which is not ideal
                throw new InvalidOperationException($"Failed to create admin user: {ex.Message}", ex);
            }
        }
    }
}

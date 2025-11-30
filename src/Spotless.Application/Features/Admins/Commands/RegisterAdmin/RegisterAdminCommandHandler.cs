using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Application.Services;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Admins.Commands.RegisterAdmin
{
    public class RegisterAdminCommandHandler(IAuthService authService, IUnitOfWork unitOfWork, CachedAdminService cachedAdminService) : IRequestHandler<RegisterAdminCommand, Guid>
    {
        private readonly IAuthService _authService = authService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly CachedAdminService _cachedAdminService = cachedAdminService;

        public async Task<Guid> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            // Check if email already exists
            var existingAdmin = await _unitOfWork.Admins.GetByEmailAsync(request.Email);
            if (existingAdmin != null)
            {
                throw new InvalidOperationException($"Admin with email {request.Email} already exists.");
            }

            // Create Identity User with Admin role
            var userId = await _authService.CreateUserAsync(request.Email, request.Password, "Admin");

            // Create Admin entity
            var admin = new Admin(
                name: request.Name,
                email: request.Email,
                adminrole: AdminRole.Support // Default role, can be changed later
            );

            await _unitOfWork.Admins.AddAsync(admin);
            await _unitOfWork.CommitAsync();

            // Invalidate cache so the new admin appears in the list immediately
            await _cachedAdminService.InvalidateAdminCacheAsync();

            return admin.Id;
        }
    }
}

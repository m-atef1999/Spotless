using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Commands.RevokeDriverAccess
{
    public class RevokeDriverAccessCommandHandler(
        IUnitOfWork unitOfWork,
        IAuthService authService) : IRequestHandler<RevokeDriverAccessCommand>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthService _authService = authService;

        public async Task Handle(RevokeDriverAccessCommand request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId);
            if (driver == null)
            {
                throw new KeyNotFoundException($"Driver with ID {request.DriverId} not found.");
            }

            // 1. Update Driver Status to Revoked
            driver.UpdateStatus(DriverStatus.Revoked);

            // 2. Remove Driver Role from User
            // Find the user ID associated with this driver
            var userId = await _authService.GetUserIdByDriverIdAsync(request.DriverId);
            if (!string.IsNullOrEmpty(userId))
            {
                await _authService.RemoveRoleAsync(userId, UserRole.Driver.ToString());
            }

            await _unitOfWork.CommitAsync();
        }
    }
}

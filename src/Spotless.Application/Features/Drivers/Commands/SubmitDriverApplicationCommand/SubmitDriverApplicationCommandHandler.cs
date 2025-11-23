using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Spotless.Application.Features.Drivers.Commands.SubmitDriverApplicationCommand
{
    public class SubmitDriverApplicationCommandHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        IAuthService authService,
        Microsoft.Extensions.Logging.ILogger<SubmitDriverApplicationCommandHandler> logger) : IRequestHandler<SubmitDriverApplicationCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IAuthService _authService = authService;
        private readonly Microsoft.Extensions.Logging.ILogger<SubmitDriverApplicationCommandHandler> _logger = logger;

        public async Task<Guid> Handle(SubmitDriverApplicationCommand request, CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId) ?? throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");
            var existingDriver = await _unitOfWork.Drivers.GetByEmailAsync(request.Dto.Email);

            if (existingDriver != null)
            {
                if (existingDriver.Status == DriverStatus.Rejected)
                {
                    // Check for 30-day cooldown
                    // Use UpdatedAt as the rejection date (assuming it was updated when rejected)
                    var rejectionDate = existingDriver.UpdatedAt ?? DateTime.MinValue;
                    var daysSinceRejection = (DateTime.UtcNow - rejectionDate).TotalDays;

                    if (daysSinceRejection < 30)
                    {
                        var daysRemaining = Math.Ceiling(30 - daysSinceRejection);
                        throw new InvalidOperationException($"Your application was rejected. You can apply again in {daysRemaining} days.");
                    }
                }

                existingDriver.UpdateProfile(request.Dto.Name, request.Dto.Phone, request.Dto.VehicleInfo);
                existingDriver.UpdateStatus(DriverStatus.PendingApproval);
                existingDriver.MarkAsUpdated(); // Ensure UpdatedAt is set
                await _unitOfWork.Drivers.UpdateAsync(existingDriver);
                await _unitOfWork.CommitAsync();
                
                // Notify Admins
                await NotifyAdminsAsync(existingDriver.Name);
                
                return existingDriver.Id;
            }

            var newDriverApplication = new Driver(
                adminId: null,
                name: request.Dto.Name,
                email: request.Dto.Email,
                phone: request.Dto.Phone,
                vehicleInfo: request.Dto.VehicleInfo
            );


            newDriverApplication.UpdateStatus(DriverStatus.PendingApproval);


            await _unitOfWork.Drivers.AddAsync(newDriverApplication);
            await _unitOfWork.CommitAsync();

            // Notify Admins
            await NotifyAdminsAsync(newDriverApplication.Name);

            return newDriverApplication.Id;
        }

        private async Task NotifyAdminsAsync(string driverName)
        {
            try
            {
                var adminIds = await _authService.GetAdminUserIdsAsync();
                foreach (var adminId in adminIds)
                {
                    await _notificationService.SendPushNotificationAsync(adminId, "New Driver Application", $"New driver application from {driverName}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send admin notifications for Driver Application");
            }
        }
    }
}

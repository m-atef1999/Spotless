using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Spotless.Application.Features.Drivers.Commands.SubmitDriverApplicationCommand
{
    public class SubmitDriverApplicationCommandHandler(
        ICustomerRepository customerRepository,
        IRepository<DriverApplication> driverApplicationRepository,
        INotificationService notificationService,
        IAuthService authService,
        Microsoft.Extensions.Logging.ILogger<SubmitDriverApplicationCommandHandler> logger) : IRequestHandler<SubmitDriverApplicationCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly IRepository<DriverApplication> _driverApplicationRepository = driverApplicationRepository;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IAuthService _authService = authService;
        private readonly Microsoft.Extensions.Logging.ILogger<SubmitDriverApplicationCommandHandler> _logger = logger;

        public async Task<Guid> Handle(SubmitDriverApplicationCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId) ?? throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");
            
            // Check for existing application
            var existingApp = await _driverApplicationRepository.GetSingleAsync(da => da.CustomerId == request.CustomerId);

            if (existingApp != null)
            {
                if (existingApp.Status == DriverApplicationStatus.Rejected)
                {
                    // Check for 30-day cooldown
                    var rejectionDate = existingApp.UpdatedAt ?? DateTime.MinValue;
                    var daysSinceRejection = (DateTime.UtcNow - rejectionDate).TotalDays;

                    if (daysSinceRejection < 30)
                    {
                        var daysRemaining = Math.Ceiling(30 - daysSinceRejection);
                        throw new InvalidOperationException($"Your application was rejected. You can apply again in {daysRemaining} days.");
                    }
                    
                    // Re-submit existing application
                    existingApp.Resubmit(request.Dto.VehicleInfo);
                    await _driverApplicationRepository.UpdateAsync(existingApp);
                    await _driverApplicationRepository.SaveChangesAsync(cancellationToken);

                    // Notify Admins
                    await NotifyAdminsAsync(customer.Name);

                    return existingApp.Id;
                }
                
                if (existingApp.Status == DriverApplicationStatus.Submitted)
                {
                     return existingApp.Id;
                }
                
                if (existingApp.Status == DriverApplicationStatus.Approved)
                {
                     return existingApp.Id;
                }
            }

            var newDriverApplication = new DriverApplication(
                request.CustomerId,
                request.Dto.VehicleInfo
            );

            await _driverApplicationRepository.AddAsync(newDriverApplication);
            await _driverApplicationRepository.SaveChangesAsync(cancellationToken);

            // Notify Admins
            await NotifyAdminsAsync(customer.Name);

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

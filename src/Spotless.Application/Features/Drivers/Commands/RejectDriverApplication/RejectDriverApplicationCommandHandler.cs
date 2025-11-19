using MediatR;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Drivers.Commands.RejectDriverApplication
{
    public class RejectDriverApplicationCommandHandler : IRequestHandler<RejectDriverApplicationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly NotificationSettings _settings;

        public RejectDriverApplicationCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService, IOptions<NotificationSettings> settings)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _settings = settings.Value;
        }

        public async Task<Unit> Handle(RejectDriverApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _unitOfWork.OrderDriverApplications.GetByIdAsync(request.ApplicationId);
            if (application == null)
                throw new KeyNotFoundException($"Application with ID {request.ApplicationId} not found.");

            if (application.Status != Domain.Enums.OrderDriverApplicationStatus.Applied)
                throw new InvalidOperationException("Application is not in an applied state.");

            application.Reject();
            await _unitOfWork.OrderDriverApplications.UpdateAsync(application);
            await _unitOfWork.CommitAsync();

            try
            {
                var driver = await _unitOfWork.Drivers.GetByIdAsync(application.DriverId);
                if (driver != null)
                {
                    if (_settings.EnableEmailNotifications && !string.IsNullOrWhiteSpace(driver.Email))
                    {
                        await _notificationService.SendEmailNotificationAsync(driver.Email, "Application Rejected", $"Your application for order #{application.OrderId} was rejected by admin.");
                    }
                    if (_settings.EnableSmsNotifications && !string.IsNullOrWhiteSpace(driver.Phone))
                    {
                        await _notificationService.SendSmsNotificationAsync(driver.Phone!, $"Your application for order #{application.OrderId} was rejected by admin.");
                    }
                    if (_settings.EnablePushNotifications)
                    {
                        await _notificationService.SendPushNotificationAsync(driver.Id.ToString(), "Application Rejected", $"Your application for order #{application.OrderId} was rejected by admin.");
                    }
                }
            }
            catch
            {
                // ignore notification errors
            }

            return Unit.Value;
        }
    }
}
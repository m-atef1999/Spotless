using MediatR;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Application.Features.Drivers.Commands.AssignDriver;

namespace Spotless.Application.Features.Drivers.Commands.AcceptDriverApplication
{
    public class AcceptDriverApplicationCommandHandler : IRequestHandler<AcceptDriverApplicationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;
        private readonly NotificationSettings _settings;

        public AcceptDriverApplicationCommandHandler(IUnitOfWork unitOfWork, IMediator mediator, INotificationService notificationService, IOptions<NotificationSettings> settings)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _notificationService = notificationService;
            _settings = settings.Value;
        }

        public async Task<Unit> Handle(AcceptDriverApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _unitOfWork.OrderDriverApplications.GetByIdAsync(request.ApplicationId);
            if (application == null)
                throw new KeyNotFoundException($"Application with ID {request.ApplicationId} not found.");

            if (application.Status != Domain.Enums.OrderDriverApplicationStatus.Applied)
                throw new InvalidOperationException("Application is not in an applied state.");

            // Reuse AssignDriver command to perform checks and assignment
            var assignCommand = new AssignDriverCommand(application.OrderId, application.DriverId, request.AdminId);

            await _mediator.Send(assignCommand);

            // Mark this application as accepted
            application.Accept();
            await _unitOfWork.OrderDriverApplications.UpdateAsync(application);

            // Auto-reject other applications for the same order
            var others = (await _unitOfWork.OrderDriverApplications.GetByOrderIdAsync(application.OrderId))
                            .Where(a => a.Id != application.Id && a.Status == Domain.Enums.OrderDriverApplicationStatus.Applied)
                            .ToList();

            foreach (var other in others)
            {
                other.Reject();
                await _unitOfWork.OrderDriverApplications.UpdateAsync(other);

                try
                {
                    // Notify rejected driver
                    var driver = await _unitOfWork.Drivers.GetByIdAsync(other.DriverId);
                    if (driver != null)
                    {
                        if (_settings.EnableEmailNotifications && !string.IsNullOrWhiteSpace(driver.Email))
                        {
                            await _notificationService.SendEmailNotificationAsync(driver.Email, "Application Not Selected", $"Your application for order #{application.OrderId} was not selected.");
                        }
                        if (_settings.EnableSmsNotifications && !string.IsNullOrWhiteSpace(driver.Phone))
                        {
                            await _notificationService.SendSmsNotificationAsync(driver.Phone!, $"Your application for order #{application.OrderId} was not selected.");
                        }
                        if (_settings.EnablePushNotifications)
                        {
                            await _notificationService.SendPushNotificationAsync(driver.Id.ToString(), "Application Update", $"Your application for order #{application.OrderId} was not selected.");
                        }
                    }
                }
                catch
                {
                    // Swallow notification exceptions to avoid failing the accept flow
                }
            }

            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Domain.Events;

namespace Spotless.Application.Features.Orders.EventHandlers
{
    public class DriverAssignedEventHandler(INotificationService notificationService, IUnitOfWork unitOfWork, IOptions<NotificationSettings> settings, ILogger<DriverAssignedEventHandler> logger) : INotificationHandler<DriverAssignedEvent>
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly NotificationSettings _settings = settings.Value;
        private readonly ILogger<DriverAssignedEventHandler> _logger = logger;

        public async Task Handle(DriverAssignedEvent notification, CancellationToken cancellationToken)
        {
            if (!_settings.NotifyOnDriverAssigned) return;

            try
            {
                var driver = await _unitOfWork.Drivers.GetByIdAsync(notification.DriverId);
                var order = await _unitOfWork.Orders.GetByIdAsync(notification.OrderId);
                var customer = order != null ? await _unitOfWork.Customers.GetByIdAsync(order.CustomerId) : null;

                if (_settings.EnableEmailNotifications && driver?.Email != null)
                    await _notificationService.SendEmailNotificationAsync(driver.Email, "New Order Assigned", $"Order #{notification.OrderId} has been assigned to you.");

                if (_settings.EnableSmsNotifications && !string.IsNullOrWhiteSpace(driver?.Phone))
                    await _notificationService.SendSmsNotificationAsync(driver!.Phone!, $"You have been assigned order #{notification.OrderId}.");

                if (_settings.EnableWhatsAppNotifications && !string.IsNullOrWhiteSpace(driver?.Phone))
                    await _notificationService.SendWhatsAppNotificationAsync(driver!.Phone!, $"You have been assigned order #{notification.OrderId}.");

                if (_settings.EnableEmailNotifications && customer?.Email != null)
                    await _notificationService.SendEmailNotificationAsync(customer.Email, "Driver Assigned", $"A driver has been assigned to your order #{notification.OrderId}.");

                if (_settings.EnableSmsNotifications && customer?.Phone != null)
                    await _notificationService.SendSmsNotificationAsync(customer.Phone!, $"A driver has been assigned to your order #{notification.OrderId}.");

                if (_settings.EnableWhatsAppNotifications && customer?.Phone != null)
                    await _notificationService.SendWhatsAppNotificationAsync(customer.Phone!, $"A driver has been assigned to your order #{notification.OrderId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send driver assigned notification");
            }
        }
    }
}

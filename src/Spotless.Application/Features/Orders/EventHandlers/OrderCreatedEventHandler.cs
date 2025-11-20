using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Domain.Events;

namespace Spotless.Application.Features.Orders.EventHandlers
{
    public class OrderCreatedEventHandler(INotificationService notificationService, IUnitOfWork unitOfWork, IOptions<NotificationSettings> settings, ILogger<OrderCreatedEventHandler> logger) : INotificationHandler<OrderCreatedEvent>
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly NotificationSettings _settings = settings.Value;
        private readonly ILogger<OrderCreatedEventHandler> _logger = logger;

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            if (!_settings.NotifyOnOrderCreated) return;

            try
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(notification.CustomerId);
                if (customer?.Email == null) return;

                var subject = "Order Created Successfully";
                var message = $"Your order #{notification.OrderId} has been created. Total: ${notification.TotalPrice:F2}";

                if (_settings.EnableEmailNotifications)
                    await _notificationService.SendEmailNotificationAsync(customer.Email, subject, message);

                if (_settings.EnableSmsNotifications && !string.IsNullOrWhiteSpace(customer.Phone))
                {
                    await _notificationService.SendSmsNotificationAsync(customer.Phone!, $"Your order #{notification.OrderId} has been created. Total: ${notification.TotalPrice:F2}");
                }

                if (_settings.EnableWhatsAppNotifications && !string.IsNullOrWhiteSpace(customer.Phone))
                {
                    await _notificationService.SendWhatsAppNotificationAsync(customer.Phone!, $"Your order #{notification.OrderId} has been created. Total: ${notification.TotalPrice:F2}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order created notification");
            }
        }
    }
}

using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Domain.Events;

namespace Spotless.Application.Features.Orders.EventHandlers
{
    public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationSettings _settings;
        private readonly ILogger<OrderCreatedEventHandler> _logger;

        public OrderCreatedEventHandler(INotificationService notificationService, IUnitOfWork unitOfWork, IOptions<NotificationSettings> settings, ILogger<OrderCreatedEventHandler> logger)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _settings = settings.Value;
            _logger = logger;
        }

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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order created notification");
            }
        }
    }
}

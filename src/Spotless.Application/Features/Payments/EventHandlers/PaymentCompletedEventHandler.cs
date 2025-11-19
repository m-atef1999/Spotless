using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Domain.Events;

namespace Spotless.Application.Features.Payments.EventHandlers
{
    public class PaymentCompletedEventHandler : INotificationHandler<PaymentCompletedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationSettings _settings;
        private readonly ILogger<PaymentCompletedEventHandler> _logger;

        public PaymentCompletedEventHandler(INotificationService notificationService, IUnitOfWork unitOfWork, IOptions<NotificationSettings> settings, ILogger<PaymentCompletedEventHandler> logger)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task Handle(PaymentCompletedEvent notification, CancellationToken cancellationToken)
        {
            if (!_settings.NotifyOnPaymentCompleted) return;

            try
            {
                if (!notification.OrderId.HasValue) return;

                var order = await _unitOfWork.Orders.GetByIdAsync(notification.OrderId.Value);
                var customer = order != null ? await _unitOfWork.Customers.GetByIdAsync(order.CustomerId) : null;

                if (_settings.EnableEmailNotifications && customer?.Email != null)
                {
                    var subject = "Payment Received - Receipt";
                    var message = $"Payment of ${notification.Amount:F2} for order #{notification.OrderId} has been completed successfully.";
                    await _notificationService.SendEmailNotificationAsync(customer.Email, subject, message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send payment completed notification");
            }
        }
    }
}

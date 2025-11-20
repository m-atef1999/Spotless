using Microsoft.Extensions.Logging;
using Spotless.Application.Interfaces;
using Spotless.Domain.Events;

namespace Spotless.Infrastructure.Services
{
    public class DomainEventPublisher(
        INotificationService notificationService,
        IAnalyticsService analyticsService,
        Spotless.Application.Interfaces.IRealTimeNotifier realTimeNotifier,
        ILogger<DomainEventPublisher> logger) : IDomainEventPublisher
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly IAnalyticsService _analyticsService = analyticsService;
        private readonly Spotless.Application.Interfaces.IRealTimeNotifier _realTimeNotifier = realTimeNotifier;
        private readonly ILogger<DomainEventPublisher> _logger = logger;

        public async Task PublishAsync<T>(T domainEvent) where T : IDomainEvent
        {
            _logger.LogInformation("Publishing domain event: {EventType} with ID: {EventId}", 
                typeof(T).Name, domainEvent.Id);

            switch (domainEvent)
            {
                case OrderCreatedEvent orderCreated:
                    await HandleOrderCreatedAsync(orderCreated);
                    break;
                case PaymentCompletedEvent paymentCompleted:
                    await HandlePaymentCompletedAsync(paymentCompleted);
                    break;
                case DriverAssignedEvent driverAssigned:
                    await HandleDriverAssignedAsync(driverAssigned);
                    break;
                default:
                    _logger.LogWarning("No handler found for event type: {EventType}", typeof(T).Name);
                    break;
            }
        }

        private async Task HandleOrderCreatedAsync(OrderCreatedEvent orderCreated)
        {
            await _analyticsService.TrackOrderCreatedAsync(orderCreated.OrderId, orderCreated.CustomerId, orderCreated.TotalPrice);
        }

        private async Task HandlePaymentCompletedAsync(PaymentCompletedEvent paymentCompleted)
        {
            await _analyticsService.TrackPaymentCompletedAsync(paymentCompleted.PaymentId, paymentCompleted.CustomerId, paymentCompleted.Amount);
        }

        private async Task HandleDriverAssignedAsync(DriverAssignedEvent driverAssigned)
        {
            await _analyticsService.TrackDriverAssignedAsync(driverAssigned.OrderId, driverAssigned.DriverId);

            try
            {
                await _realTimeNotifier.NotifyDriverAssignedAsync(driverAssigned.DriverId, driverAssigned.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send real-time DriverAssigned notification for Order {OrderId} to Driver {DriverId}", driverAssigned.OrderId, driverAssigned.DriverId);
            }
        }
    }
}
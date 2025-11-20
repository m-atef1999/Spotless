using Microsoft.Extensions.Logging;
using Spotless.Application.Interfaces;

namespace Spotless.Infrastructure.Services
{
    public class AnalyticsService(ILogger<AnalyticsService> logger) : IAnalyticsService
    {
        private readonly ILogger<AnalyticsService> _logger = logger;

        public async Task TrackOrderCreatedAsync(Guid orderId, Guid customerId, decimal amount)
        {
            _logger.LogInformation("Analytics: Order created - OrderId: {OrderId}, CustomerId: {CustomerId}, Amount: {Amount}", 
                orderId, customerId, amount);
            await Task.CompletedTask;
        }

        public async Task TrackPaymentCompletedAsync(Guid paymentId, Guid customerId, decimal amount)
        {
            _logger.LogInformation("Analytics: Payment completed - PaymentId: {PaymentId}, CustomerId: {CustomerId}, Amount: {Amount}", 
                paymentId, customerId, amount);
            await Task.CompletedTask;
        }

        public async Task TrackDriverAssignedAsync(Guid orderId, Guid driverId)
        {
            _logger.LogInformation("Analytics: Driver assigned - OrderId: {OrderId}, DriverId: {DriverId}", 
                orderId, driverId);
            await Task.CompletedTask;
        }

        public async Task TrackUserBehaviorAsync(string userId, string action, Dictionary<string, object>? properties = null)
        {
            _logger.LogInformation("Analytics: User behavior - UserId: {UserId}, Action: {Action}, Properties: {Properties}", 
                userId, action, properties);
            await Task.CompletedTask;
        }
    }
}
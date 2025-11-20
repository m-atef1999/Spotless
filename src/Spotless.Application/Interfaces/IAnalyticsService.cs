namespace Spotless.Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task TrackOrderCreatedAsync(Guid orderId, Guid customerId, decimal amount);
        Task TrackPaymentCompletedAsync(Guid paymentId, Guid customerId, decimal amount);
        Task TrackDriverAssignedAsync(Guid orderId, Guid driverId);
        Task TrackUserBehaviorAsync(string userId, string action, Dictionary<string, object>? properties = null);
    }
}

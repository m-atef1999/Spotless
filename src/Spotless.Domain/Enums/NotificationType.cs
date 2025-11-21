namespace Spotless.Domain.Enums
{
    public enum NotificationType
    {
        OrderCreated,
        OrderConfirmed,
        OrderAssigned,
        OrderInProgress,
        OrderCompleted,
        OrderCancelled,
        PaymentReceived,
        PaymentFailed,
        DriverApplicationApproved,
        DriverApplicationRejected,
        System,
        Promotion
    }
}

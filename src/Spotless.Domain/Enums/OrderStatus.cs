namespace Spotless.Domain.Enums
{
    public enum OrderStatus
    {
        PaymentFailed,
        Requested,
        Confirmed,
        DriverAssigned,
        PickedUp,
        InCleaning,
        OutForDelivery,
        Delivered,
        Cancelled
    }
}

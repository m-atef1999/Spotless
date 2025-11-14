using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Order
{
    public record OrderDto(
        Guid Id,
        Guid CustomerId,
        Guid? DriverId,

        Guid TimeSlotId,
        DateTime ScheduledDate,

        decimal PickupLatitude,
        decimal PickupLongitude,
        decimal DeliveryLatitude,
        decimal DeliveryLongitude,

        decimal TotalPrice,
        string Currency,
        OrderStatus Status,
        PaymentMethod PaymentMethod,
        DateTime OrderDate,
        IReadOnlyList<OrderItemDto> Items
    );
}

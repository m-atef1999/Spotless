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
        string? PickupAddress,
        decimal DeliveryLatitude,
        decimal DeliveryLongitude,
        string? DeliveryAddress,

        decimal TotalPrice,
        string Currency,
        OrderStatus Status,
        PaymentMethod PaymentMethod,
        DateTime OrderDate,
        IReadOnlyList<OrderItemDto> Items
    );
}

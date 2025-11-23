using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Order
{
    public record OrderDto(
        Guid Id,
        Guid CustomerId,
        Guid? DriverId,

        Guid TimeSlotId,
        TimeSpan? StartTime,
        TimeSpan? EndTime,
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
        string ServiceName,
        DateTime CreatedAt,
        DateTime OrderDate,
        decimal EstimatedDurationHours,
        IReadOnlyList<OrderItemDto> Items
    );
}

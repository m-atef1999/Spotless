using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Order
{
    public record CreateOrderDto(
        Guid TimeSlotId,
        DateTime ScheduledDate,
        PaymentMethod PaymentMethod,

        decimal PickupLatitude,
        decimal PickupLongitude,
        string? PickupAddress,
        decimal DeliveryLatitude,
        decimal DeliveryLongitude,
        string? DeliveryAddress,

        IReadOnlyList<CreateOrderItemDto> Items
    );
}

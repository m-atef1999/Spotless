using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Order
{
    public record CreateOrderDto(
        Guid TimeSlotId,
        DateTime ScheduledDate,
        PaymentMethod PaymentMethod,

        decimal PickupLatitude,
        decimal PickupLongitude,
        decimal DeliveryLatitude,
        decimal DeliveryLongitude,

        IReadOnlyList<CreateOrderItemDto> Items
    );
}
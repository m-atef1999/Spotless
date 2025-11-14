using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Order
{
    public record CreateOrderDto(
        IReadOnlyList<OrderItemDto> Items,
        Guid TimeSlotId,
        DateTime ScheduledDate,
        PaymentMethod PaymentMethod,
        decimal PickupLatitude,
        decimal PickupLongitude,
        decimal DeliveryLatitude,
        decimal DeliveryLongitude
    );
}

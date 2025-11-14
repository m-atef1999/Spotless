namespace Spotless.Application.Dtos.Order
{
    public record UpdateOrderDto(
        Guid ServiceId,
        DateTime PickupTime,
        DateTime DeliveryTime
    );
}

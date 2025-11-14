using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Dtos.Order
{
    public record CreateOrderDto(
        Guid ServiceId,
        Address PickupAddress,
        Address DeliveryAddress,
        DateTime PickupTime,
        DateTime DeliveryTime,
        PaymentMethod PaymentMethod);
}

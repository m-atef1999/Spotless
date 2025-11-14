using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Order
{
    public record OrderDto(
        Guid Id,
        Guid CustomerId,
        Guid ServiceId,
        Guid? DriverId,
        decimal TotalPrice,
        string Currency,
        DateTime PickupTime,
        DateTime DeliveryTime,
        OrderStatus Status,
        PaymentMethod PaymentMethod,
        DateTime OrderDate
    );
}

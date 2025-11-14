namespace Spotless.Application.Dtos.Order
{
    public record OrderDto(
        Guid Id,
        Guid CustomerId,
        Guid ServiceId,
        string ServiceName,
        string TotalPrice,
        string Status,
        DateTime OrderDate,
        DateTime DeliveryTime);
}

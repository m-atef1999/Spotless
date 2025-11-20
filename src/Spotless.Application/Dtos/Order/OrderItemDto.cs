namespace Spotless.Application.Dtos.Order
{
    public record OrderItemDto(
            Guid Id,
            Guid ServiceId,
            decimal PriceAmount,
            string PriceCurrency,
            int Quantity
        );
}

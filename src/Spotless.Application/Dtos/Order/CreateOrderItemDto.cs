namespace Spotless.Application.Dtos.Order
{
    public record CreateOrderItemDto(
            Guid ServiceId,
            string ItemName,
            int Quantity
        );
}

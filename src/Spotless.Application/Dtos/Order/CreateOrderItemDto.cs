namespace Spotless.Application.Dtos.Order
{
    public record CreateOrderItemDto(
            Guid ServiceId,
            int Quantity
        );
}

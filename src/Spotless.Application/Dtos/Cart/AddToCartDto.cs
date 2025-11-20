namespace Spotless.Application.Dtos.Cart
{
    public record AddToCartDto(
        Guid ServiceId,
        int Quantity
    );
}

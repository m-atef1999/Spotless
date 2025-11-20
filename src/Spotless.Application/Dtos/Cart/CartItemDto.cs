namespace Spotless.Application.Dtos.Cart
{
    public record CartItemDto(
        Guid Id,
        Guid ServiceId,
        string ServiceName,
        int Quantity,
        decimal MaxWeightKg,
        DateTime AddedDate
    );
}

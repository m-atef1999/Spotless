namespace Spotless.Application.Dtos.Cart
{
    public record CartDto(
        Guid Id,
        Guid CustomerId,
        IReadOnlyList<CartItemDto> Items,
        decimal TotalWeightKg,
        DateTime CreatedDate,
        DateTime LastModifiedDate
    );
}

using Spotless.Application.Dtos.Order;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Interfaces
{
    public interface IPricingService
    {
        Task<IReadOnlyList<PriceCalculationResult>> GetItemPricesAsync(IReadOnlyList<OrderItemDto> items);
        Money CalculateTotal(IReadOnlyList<PriceCalculationResult> itemPrices);

    }
    public record PriceCalculationResult(
    Guid ServiceId,
    Money Price
);
}

using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Interfaces
{

    public record PriceCalculationResult(
        Guid ServiceId,
        Money Price
    );

    public interface IPricingService
    {

        Task<IReadOnlyList<PriceCalculationResult>> GetItemPricesAsync(IReadOnlyList<PricingItemDto> items);


        PriceEstimateDto CalculateTotal(IReadOnlyList<PriceCalculationResult> itemPrices);
    }
}
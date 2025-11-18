using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Orders.Queries.GetPriceEstimate
{
    public record GetPriceEstimateQuery(
        PricingDetailsDto Details
    ) : IQuery<PriceEstimateDto>;
}
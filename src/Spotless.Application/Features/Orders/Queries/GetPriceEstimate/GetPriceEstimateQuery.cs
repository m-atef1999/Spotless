using MediatR;

namespace Spotless.Application.Features.Orders.Queries.GetPriceEstimate
{
    public record GetPriceEstimateQuery(
        PricingDetailsDto Details
    ) : IRequest<PriceEstimateDto>;
}
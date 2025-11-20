using MediatR;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Orders.Queries.GetPriceEstimate
{
    public class GetPriceEstimateQueryHandler(IPricingService pricingService) : IRequestHandler<GetPriceEstimateQuery, PriceEstimateResponse>
    {
        private readonly IPricingService _pricingService = pricingService;

        public async Task<PriceEstimateResponse> Handle(GetPriceEstimateQuery request, CancellationToken cancellationToken)
        {
            return await _pricingService.CalculatePriceEstimateAsync(request.OrderDto, request.CustomerId, cancellationToken);
        }
    }
}
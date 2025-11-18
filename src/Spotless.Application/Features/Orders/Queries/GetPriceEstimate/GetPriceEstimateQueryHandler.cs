using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Orders.Queries.GetPriceEstimate
{


    public class GetPriceEstimateQueryHandler : IRequestHandler<GetPriceEstimateQuery, PriceEstimateDto>
    {
        private readonly IPricingService _pricingService;

        public GetPriceEstimateQueryHandler(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        public async Task<PriceEstimateDto> Handle(GetPriceEstimateQuery request, CancellationToken cancellationToken)
        {

            var itemPrices = await _pricingService.GetItemPricesAsync(request.Details.ServiceItems);

            var estimate = _pricingService.CalculateTotal(itemPrices);

            return estimate;
        }
    }
}
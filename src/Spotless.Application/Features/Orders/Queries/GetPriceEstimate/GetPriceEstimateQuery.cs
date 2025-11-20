using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Orders.Queries.GetPriceEstimate
{
    public record GetPriceEstimateQuery(
        CreateOrderDto OrderDto,
        Guid CustomerId
    ) : IRequest<PriceEstimateResponse>;
}

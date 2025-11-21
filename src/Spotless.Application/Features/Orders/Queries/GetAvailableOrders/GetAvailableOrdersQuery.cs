using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Responses;

namespace Spotless.Application.Features.Orders.Queries.GetAvailableOrders
{
    public record GetAvailableOrdersQuery(
        int PageNumber,
        int PageSize) : IRequest<PagedResponse<OrderDto>>;
}

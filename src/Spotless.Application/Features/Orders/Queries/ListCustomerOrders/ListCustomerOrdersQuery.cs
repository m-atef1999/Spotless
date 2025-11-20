using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Responses;

namespace Spotless.Application.Features.Orders.Queries.ListCustomerOrders
{
    public record ListCustomerOrdersQuery(
        Guid CustomerId,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<PagedResponse<OrderDto>>;
}

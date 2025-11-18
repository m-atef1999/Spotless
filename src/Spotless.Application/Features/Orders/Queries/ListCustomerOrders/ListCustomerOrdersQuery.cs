using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Requests;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Queries.ListCustomerOrders
{
    public record ListCustomerOrdersQuery(
            Guid CustomerId,
            OrderStatus? StatusFilter = null
        ) : PaginationBaseRequest, IQuery<PagedResponse<OrderDto>>;
}

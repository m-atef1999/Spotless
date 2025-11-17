using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Requests;
using Spotless.Application.Dtos.Responses;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders
{
    public record ListCustomerOrdersQuery(
            Guid CustomerId,
            OrderStatus? StatusFilter = null
        ) : PaginationBaseRequest, IRequest<PagedResponse<OrderDto>>;
}

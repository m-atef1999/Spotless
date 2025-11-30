using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Responses;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Queries.GetOrdersAdmin
{
    public record GetOrdersAdminQuery(
        int PageNumber = 1,
        int PageSize = 10,
        OrderStatus? Status = null,
        string? SearchTerm = null
    ) : IRequest<PagedResponse<OrderDto>>;
}

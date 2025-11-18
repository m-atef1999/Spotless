using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Requests;
using Spotless.Application.Dtos.Responses;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Queries.GetOrderQuery
{

    public record GetOrdersQuery(

        string? CustomerEmail,
        OrderStatus? StatusFilter

    ) : PaginationBaseRequest, IRequest<PagedResponse<OrderDto>>;
}
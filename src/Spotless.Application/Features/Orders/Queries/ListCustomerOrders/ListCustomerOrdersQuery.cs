using MediatR;
using Spotless.Application.Dtos.Order;

namespace Spotless.Application.Features.Orders.Queries.ListCustomerOrders
{
    public record ListCustomerOrdersQuery(Guid CustomerId) : IRequest<IReadOnlyList<OrderDto>>;
}

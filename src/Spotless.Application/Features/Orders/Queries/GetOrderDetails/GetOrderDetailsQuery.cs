using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Orders.Queries.GetOrderDetails
{
    public record GetOrderDetailsQuery(Guid OrderId) : IQuery<OrderDto>;
}

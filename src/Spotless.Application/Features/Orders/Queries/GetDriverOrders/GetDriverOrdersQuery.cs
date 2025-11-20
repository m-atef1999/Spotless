using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Orders.Queries.GetDriverOrders
{
    public record GetDriverOrdersQuery(Guid DriverId) : IQuery<IReadOnlyList<OrderDto>>;
}

using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Orders.Queries.GetAvailableForDrivers
{
    public record GetAvailableForDriversQuery() : IQuery<IReadOnlyList<OrderDto>>;
}
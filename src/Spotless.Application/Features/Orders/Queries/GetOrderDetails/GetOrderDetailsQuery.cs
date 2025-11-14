using MediatR;
using Spotless.Application.Dtos.Order;

namespace Spotless.Application.Features.Customers.Queries.GetOrderDetails
{
    public record GetOrderDetailsQuery(Guid OrderId) : IRequest<OrderDto>;
}

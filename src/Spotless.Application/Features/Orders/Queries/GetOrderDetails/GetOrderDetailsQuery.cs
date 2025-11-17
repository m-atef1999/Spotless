using MediatR;
using Spotless.Application.Dtos.Order;

namespace Spotless.Application.Features.Orders
{
    public record GetOrderDetailsQuery(Guid OrderId) : IRequest<OrderDto>;
}

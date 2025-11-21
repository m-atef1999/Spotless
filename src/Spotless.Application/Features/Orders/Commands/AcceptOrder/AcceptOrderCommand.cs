using MediatR;
using Spotless.Application.Dtos.Order;

namespace Spotless.Application.Features.Orders.Commands.AcceptOrder
{
    public record AcceptOrderCommand(Guid OrderId, Guid DriverId) : IRequest<OrderDto>;
}

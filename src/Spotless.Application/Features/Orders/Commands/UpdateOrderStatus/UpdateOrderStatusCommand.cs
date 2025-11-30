using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus NewStatus) : IRequest<OrderDto>;
}

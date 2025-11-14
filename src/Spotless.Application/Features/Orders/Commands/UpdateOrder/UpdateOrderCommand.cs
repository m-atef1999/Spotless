using MediatR;
using Spotless.Application.Dtos.Order;

namespace Spotless.Application.Features.Orders.Commands.UpdateOrder
{
    public record UpdateOrderCommand(
        Guid OrderId,
        UpdateOrderDto Dto,
        Guid CustomerId) : IRequest<Unit>;
}

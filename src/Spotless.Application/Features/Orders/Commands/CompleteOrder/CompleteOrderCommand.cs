using MediatR;

namespace Spotless.Application.Features.Orders.Commands.CompleteOrder
{
    public record CompleteOrderCommand(Guid OrderId) : IRequest<Unit>;
}

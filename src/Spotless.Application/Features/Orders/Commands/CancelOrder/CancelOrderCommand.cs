using MediatR;

namespace Spotless.Application.Features.Orders.Commands.CancelOrder
{

    public record CancelOrderCommand(
        Guid OrderId,
        Guid CustomerId
    ) : IRequest<Unit>;
}
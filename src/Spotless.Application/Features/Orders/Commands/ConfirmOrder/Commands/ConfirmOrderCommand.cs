using MediatR;

namespace Spotless.Application.Features.Orders.Commands.ConfirmOrder.Commands
{
    public record ConfirmOrderCommand(
        Guid OrderId,
        Guid AdminId
    ) : IRequest<Unit>;
}

using MediatR;
using Spotless.Application.Dtos.Order;

namespace Spotless.Application.Features.Orders.Commands.CancelOrder
{
    public record CancelOrderCommand(
        Guid OrderId,
        Guid CustomerId
    ) : IRequest<Unit>;
}
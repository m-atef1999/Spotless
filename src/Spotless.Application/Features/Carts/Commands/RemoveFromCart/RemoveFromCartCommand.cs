using MediatR;

namespace Spotless.Application.Features.Carts.Commands.RemoveFromCart
{
    public record RemoveFromCartCommand(Guid CustomerId, Guid ServiceId) : IRequest<Unit>;
}

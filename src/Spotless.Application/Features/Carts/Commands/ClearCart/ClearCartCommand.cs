using MediatR;

namespace Spotless.Application.Features.Carts.Commands.ClearCart
{
    public record ClearCartCommand(Guid CustomerId) : IRequest<Unit>;
}

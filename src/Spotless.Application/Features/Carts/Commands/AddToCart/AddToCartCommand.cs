using MediatR;
using Spotless.Application.Dtos.Cart;

namespace Spotless.Application.Features.Carts.Commands.AddToCart
{
    public record AddToCartCommand(Guid CustomerId, AddToCartDto Dto) : IRequest<Unit>;
}

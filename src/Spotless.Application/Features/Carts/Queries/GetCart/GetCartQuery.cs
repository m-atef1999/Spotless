using MediatR;
using Spotless.Application.Dtos.Cart;

namespace Spotless.Application.Features.Carts.Queries.GetCart
{
    public record GetCartQuery(Guid CustomerId) : IRequest<CartDto?>;
}

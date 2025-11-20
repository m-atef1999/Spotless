using MediatR;
using Spotless.Application.Dtos.Cart;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Carts.Queries.GetCart
{
    public class GetCartQueryHandler(ICartService cartService) : IRequestHandler<GetCartQuery, CartDto?>
    {
        private readonly ICartService _cartService = cartService;

        public async Task<CartDto?> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            return await _cartService.GetCartAsync(request.CustomerId);
        }
    }
}

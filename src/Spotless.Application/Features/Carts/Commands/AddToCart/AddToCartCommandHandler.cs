using MediatR;
using Spotless.Application.Dtos.Cart;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Carts.Commands.AddToCart
{
    public class AddToCartCommandHandler(ICartService cartService) : IRequestHandler<AddToCartCommand, Unit>
    {
        private readonly ICartService _cartService = cartService;

        public async Task<Unit> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            await _cartService.AddToCartAsync(request.CustomerId, request.Dto);
            return Unit.Value;
        }
    }
}

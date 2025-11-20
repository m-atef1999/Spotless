using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Carts.Commands.ClearCart
{
    public class ClearCartCommandHandler(ICartService cartService) : IRequestHandler<ClearCartCommand, Unit>
    {
        private readonly ICartService _cartService = cartService;

        public async Task<Unit> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            await _cartService.ClearCartAsync(request.CustomerId);
            return Unit.Value;
        }
    }
}

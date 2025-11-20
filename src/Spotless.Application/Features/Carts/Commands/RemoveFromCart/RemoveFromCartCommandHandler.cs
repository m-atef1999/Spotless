using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Carts.Commands.RemoveFromCart
{
    public class RemoveFromCartCommandHandler(ICartService cartService) : IRequestHandler<RemoveFromCartCommand, Unit>
    {
        private readonly ICartService _cartService = cartService;

        public async Task<Unit> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
        {
            await _cartService.RemoveFromCartAsync(request.CustomerId, request.ServiceId);
            return Unit.Value;
        }
    }
}

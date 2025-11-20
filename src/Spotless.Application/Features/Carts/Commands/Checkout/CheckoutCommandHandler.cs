using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Application.Features.Carts.Commands.Checkout;

namespace Spotless.Application.Features.Carts.Commands.Checkout
{
    public class CheckoutCommandHandler(ICartService cartService, IMediator mediator) : IRequestHandler<CheckoutCommand, Guid>
    {
        private readonly ICartService _cartService = cartService;
        private readonly IMediator _mediator = mediator;

        public async Task<Guid> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartService.GetCartAsync(request.CustomerId);
            if (cart == null || !cart.Items.Any())
                throw new InvalidOperationException("Cart is empty.");

            // Build CreateOrderDto from cart
            var createItems = cart.Items.Select(i => new CreateOrderItemDto(i.ServiceId, i.ServiceName, i.Quantity)).ToList();

            var createOrderDto = new CreateOrderDto(
                request.TimeSlotId,
                request.ScheduledDate,
                request.PaymentMethod,
                request.PickupLatitude,
                request.PickupLongitude,
                request.DeliveryLatitude,
                request.DeliveryLongitude,
                createItems
            );

            // Use existing CreateOrderCommand to create the order
            var createOrderCommand = new Spotless.Application.Features.Orders.Commands.CreateOrder.CreateOrderCommand(createOrderDto, request.CustomerId);

            var createdOrder = await _mediator.Send(createOrderCommand, cancellationToken);
            var orderId = createdOrder.Id;

            // Clear the cart after successful checkout
            await _cartService.ClearCartAsync(request.CustomerId);

            return orderId;
        }
    }
}

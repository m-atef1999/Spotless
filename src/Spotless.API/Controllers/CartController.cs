using Microsoft.AspNetCore.Mvc;
using MediatR;
using Spotless.Application.Dtos.Cart;
using Spotless.Application.Features.Carts.Commands.AddToCart;
using Spotless.Application.Features.Carts.Commands.RemoveFromCart;
using Spotless.Application.Features.Carts.Commands.ClearCart;
using Spotless.Application.Features.Carts.Queries.GetCart;
using Spotless.Application.Features.Carts.Commands.Checkout;
using Spotless.API.Dtos.Cart;
using Spotless.Application.Features.Orders.Commands.CreateOrder;
using Spotless.Application.Dtos.Order;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCart(Guid customerId)
        {
            var cart = await _mediator.Send(new GetCartQuery(customerId));
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpPost("{customerId}/items")]
        public async Task<IActionResult> AddToCart(Guid customerId, [FromBody] AddToCartDto dto)
        {
            await _mediator.Send(new AddToCartCommand(customerId, dto));
            return NoContent();
        }

        [HttpDelete("{customerId}/items/{serviceId}")]
        public async Task<IActionResult> RemoveFromCart(Guid customerId, Guid serviceId)
        {
            await _mediator.Send(new RemoveFromCartCommand(customerId, serviceId));
            return NoContent();
        }

        [HttpDelete("{customerId}")]
        public async Task<IActionResult> ClearCart(Guid customerId)
        {
            await _mediator.Send(new ClearCartCommand(customerId));
            return NoContent();
        }

        [HttpPost("{customerId}/checkout")]
        public async Task<IActionResult> Checkout(Guid customerId, [FromBody] CartCheckoutRequest req)
        {
            var cmd = new CheckoutCommand(customerId, req.TimeSlotId, req.ScheduledDate, req.PaymentMethod,
                req.PickupLatitude, req.PickupLongitude, req.DeliveryLatitude, req.DeliveryLongitude);

            var orderId = await _mediator.Send(cmd);
            return CreatedAtAction(null, new { id = orderId }, new { orderId });
        }

        [HttpPost("{customerId}/buynow")]
        public async Task<IActionResult> BuyNow(Guid customerId, [FromBody] BuyNowRequest req)
        {
            var items = new List<CreateOrderItemDto>
            {
                new(req.ServiceId, "", req.Quantity)
            };

            var createOrderDto = new CreateOrderDto(
                req.TimeSlotId,
                req.ScheduledDate,
                req.PaymentMethod,
                req.PickupLatitude,
                req.PickupLongitude,
                req.DeliveryLatitude,
                req.DeliveryLongitude,
                items
            );

            var cmd = new CreateOrderCommand(createOrderDto, customerId);
            var order = await _mediator.Send(cmd);

            return CreatedAtAction(null, new { id = order.Id }, order);
        }
    }
}

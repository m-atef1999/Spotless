using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Cart;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Features.Carts.Commands.AddToCart;
using Spotless.Application.Features.Carts.Commands.Checkout;
using Spotless.Application.Features.Carts.Commands.ClearCart;
using Spotless.Application.Features.Carts.Commands.RemoveFromCart;
using Spotless.Application.Features.Carts.Queries.GetCart;
using Spotless.Application.Features.Orders.Commands.CreateOrder;
using Spotless.Infrastructure.Identity;
using System.Security.Claims;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/customers/cart")]
    [Authorize]
    public class CartsController(IMediator mediator, UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        private async Task<Guid> GetCurrentCustomerIdAsync()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("User ID not found in claims.");

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.CustomerId.HasValue)
                throw new UnauthorizedAccessException("Customer profile not found.");

            return user.CustomerId.Value;
        }

        
        
        /// <summary>
        /// Retrieves authenticated customer's shopping cart
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var customerId = await GetCurrentCustomerIdAsync();
            var cart = await _mediator.Send(new GetCartQuery(customerId));
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        /// <summary>
        /// Adds a service to customer's cart
        /// </summary>
        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var customerId = await GetCurrentCustomerIdAsync();
            await _mediator.Send(new AddToCartCommand(customerId, dto));
            return NoContent();
        }

        /// <summary>
        /// Removes a service from customer's cart
        /// </summary>
        [HttpDelete("items/{serviceId}")]
        public async Task<IActionResult> RemoveFromCart(Guid serviceId)
        {
            var customerId = await GetCurrentCustomerIdAsync();
            await _mediator.Send(new RemoveFromCartCommand(customerId, serviceId));
            return NoContent();
        }

        /// <summary>
        /// Clears all items from customer's cart
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var customerId = await GetCurrentCustomerIdAsync();
            await _mediator.Send(new ClearCartCommand(customerId));
            return NoContent();
        }

        /// <summary>
        /// Checks out cart and creates an order
        /// </summary>
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CartCheckoutRequest req)
        {
            var customerId = await GetCurrentCustomerIdAsync();
            var cmd = new CheckoutCommand(customerId, req.TimeSlotId, req.ScheduledDate, req.PaymentMethod,
                req.PickupLatitude, req.PickupLongitude, req.PickupAddress,
                req.DeliveryLatitude, req.DeliveryLongitude, req.DeliveryAddress);

            var orderId = await _mediator.Send(cmd);
            return CreatedAtAction(null, new { id = orderId }, new { orderId });
        }

        /// <summary>
        /// Creates immediate order without cart
        /// </summary>
        [HttpPost("buy-now")]
        public async Task<IActionResult> BuyNow([FromBody] BuyNowRequest req)
        {
            var customerId = await GetCurrentCustomerIdAsync();
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
                req.PickupAddress,
                req.DeliveryLatitude,
                req.DeliveryLongitude,
                req.DeliveryAddress,
                items
            );

            var cmd = new CreateOrderCommand(createOrderDto, customerId);
            var order = await _mediator.Send(cmd);

            return CreatedAtAction(null, new { id = order.Id }, order);
        }
    }
}

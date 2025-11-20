using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Features.Orders.Commands.CreateOrder;
using Spotless.Application.Features.Orders.Commands.CancelOrder;
using Spotless.Application.Features.Orders.Queries.GetOrderDetails;
using Spotless.Application.Features.Orders.Queries.ListCustomerOrders;
using Spotless.Domain.Exceptions;
using System.Security.Claims;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var customerId = GetCurrentUserId();
            var command = new CreateOrderCommand(dto, customerId);
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var query = new GetOrderDetailsQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("customer")]
        [ProducesResponseType(typeof(PagedResponse<OrderDto>), 200)]
        public async Task<IActionResult> ListCustomerOrders(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            var customerId = GetCurrentUserId();
            pageNumber ??= 1;
            pageSize ??= 10;

            var query = new ListCustomerOrdersQuery(customerId, pageNumber.Value, pageSize.Value);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("{id}/cancel")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var customerId = GetCurrentUserId();
            var command = new CancelOrderCommand(id, customerId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString))
            {

                throw new UnauthorizedException("User identity claim is missing or empty. Authentication token is invalid or incomplete.");
            }

                if (Guid.TryParse(userIdString, out Guid userId))
                {
                    return userId;
            }
            else
            {

                throw new NotFoundException($"User ID claim value '{userIdString}' is not a valid identifier.");
            }
        }
    }
}

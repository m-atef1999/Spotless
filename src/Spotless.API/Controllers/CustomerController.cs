using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Customer;
using Spotless.Application.Features.Customers.Queries.GetCustomerDashboard;
using Spotless.Infrastructure.Identity;
using System.Security.Claims;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDashboardDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDashboard()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });
            }

            var user = await _userManager.FindByIdAsync(userIdString);

            if (user == null || !user.CustomerId.HasValue)
            {
                return NotFound(new { Message = "Customer profile not found for this user." });
            }

            var query = new GetCustomerDashboardQuery(user.CustomerId.Value);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}


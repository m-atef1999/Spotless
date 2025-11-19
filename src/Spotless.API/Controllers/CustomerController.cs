using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Customer;
using Spotless.Application.Features.Customers.Queries.GetCustomerDashboard;
using Spotless.Application.Interfaces;
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
        private readonly IPaginationService _paginationService;

        public CustomerController(
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            IPaginationService paginationService)
        {
            _mediator = mediator;
            _userManager = userManager;
            _paginationService = paginationService;
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDashboardDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDashboard(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.CustomerId.HasValue)
                return NotFound(new { Message = "Customer profile not found for this user." });

            pageNumber ??= _paginationService.GetDefaultPageNumber();
            pageSize = _paginationService.NormalizePageSize(pageSize);

            var query = new GetCustomerDashboardQuery(
                user.CustomerId.Value,
                pageNumber.Value,
                pageSize.Value);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Spotless.Application.Dtos.Customer.CustomerDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.CustomerId.HasValue)
                return NotFound(new { Message = "Customer profile not found for this user." });

            var query = new Spotless.Application.Features.Customers.Queries.GetCustomerProfile.GetCustomerProfileQuery(user.CustomerId.Value);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProfile([FromBody] Spotless.Application.Dtos.Customer.UpdateCustomerDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.CustomerId.HasValue)
                return NotFound(new { Message = "Customer profile not found for this user." });

            var command = new Spotless.Application.Features.Customers.Commands.UpdateCustomerProfile.UpdateCustomerProfileCommand(dto, user.CustomerId.Value);

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPost("topup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> TopUpWallet([FromBody] Spotless.Application.Dtos.Customer.TopUpWalletRequest dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.CustomerId.HasValue)
                return NotFound(new { Message = "Customer profile not found for this user." });

            var command = new Spotless.Application.Features.Customers.Commands.TopUpWallet.TopUpWalletCommand(user.CustomerId.Value, dto);

            await _mediator.Send(command);

            return Ok(new { Message = "Wallet successfully topped up." });
        }
    }
}

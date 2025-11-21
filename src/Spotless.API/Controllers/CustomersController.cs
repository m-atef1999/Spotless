using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Customer;
using Spotless.Application.Features.Customers.Commands.RegisterCustomer;
using Spotless.Application.Features.Customers.Queries.GetCustomerDashboard;
using Spotless.Application.Interfaces;
using Spotless.Infrastructure.Identity;
using System.Security.Claims;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/customers")]
    [Authorize]
    public class CustomersController(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        IPaginationService paginationService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IPaginationService _paginationService = paginationService;

        /// <summary>
        /// Lists all customers (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Spotless.Application.Dtos.Responses.PagedResponse<Spotless.Application.Dtos.Customer.CustomerDto>), 200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListCustomers(
            [FromQuery] string? nameFilter,
            [FromQuery] string? emailFilter,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            pageNumber ??= _paginationService.GetDefaultPageNumber();
            pageSize = _paginationService.NormalizePageSize(pageSize);

            var query = new Spotless.Application.Features.Customers.Queries.GetAllCustomers.ListCustomersQuery(nameFilter, emailFilter)
            {
                PageNumber = pageNumber.Value,
                PageSize = pageSize.Value
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Registers a new customer account
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Spotless.Application.Dtos.Authentication.AuthResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCustomerCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves customer dashboard with orders and wallet info
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDashboardDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDashboard(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
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

        /// <summary>
        /// Retrieves authenticated customer's profile
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Spotless.Application.Dtos.Customer.CustomerDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.CustomerId.HasValue)
                return NotFound(new { Message = "Customer profile not found for this user." });

            var query = new Spotless.Application.Features.Customers.Queries.GetCustomerProfile.GetCustomerProfileQuery(user.CustomerId.Value);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        /// Updates authenticated customer's profile
        /// </summary>
        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProfile([FromBody] Spotless.Application.Dtos.Customer.CustomerUpdateRequest dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.CustomerId.HasValue)
                return NotFound(new { Message = "Customer profile not found for this user." });

            var command = new Spotless.Application.Features.Customers.Commands.UpdateCustomerProfile.UpdateCustomerProfileCommand(dto, user.CustomerId.Value);

            await _mediator.Send(command);

            return NoContent();
        }

        /// <summary>
        /// Tops up customer's wallet balance
        /// </summary>
        [HttpPost("topup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> TopUpWallet([FromBody] Spotless.Application.Dtos.Customer.WalletTopUpRequest dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.CustomerId.HasValue)
                return NotFound(new { Message = "Customer profile not found for this user." });

            var command = new Spotless.Application.Features.Customers.Commands.TopUpWallet.TopUpWalletCommand(user.CustomerId.Value, dto);

            await _mediator.Send(command);

            return Ok(new { Message = "Wallet successfully topped up." });
        }
        /// <summary>
        /// Retrieves authenticated customer's saved payment methods
        /// </summary>
        [HttpGet("payment-methods")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Spotless.Application.Dtos.PaymentMethods.PaymentMethodDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.CustomerId.HasValue)
                return NotFound(new { Message = "Customer profile not found for this user." });

            var query = new Spotless.Application.Features.Customers.Queries.GetPaymentMethods.GetPaymentMethodsQuery(user.CustomerId.Value);

            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Features.Drivers.Commands.SubmitDriverApplicationCommand;
using Spotless.Infrastructure.Identity;
using System.Security.Claims;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DriversController(IMediator mediator, UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Register([FromBody] SubmitDriverApplicationDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var identityUserId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var identityUser = await _userManager.FindByIdAsync(userIdString);
            if (identityUser == null || !identityUser.CustomerId.HasValue)
                return NotFound(new { Message = "Customer profile not found for this user." });

            // We expect that a customer is registering as a driver (submit application)
            var command = new SubmitDriverApplicationCommand(identityUser.CustomerId.Value, dto);

            var applicationId = await _mediator.Send(command);

            return CreatedAtAction(nameof(Register), new { id = applicationId }, applicationId);
        }

        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DriverDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.DriverId.HasValue)
                return NotFound(new { Message = "Driver profile not found for this user." });

            var query = new Spotless.Application.Features.Drivers.Queries.GetDriverProfile.GetDriverProfileQuery(user.DriverId.Value);

            var dto = await _mediator.Send(query);

            return Ok(dto);
        }

        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateDriverProfileDto dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.DriverId.HasValue)
                return NotFound(new { Message = "Driver profile not found for this user." });

            var command = new Spotless.Application.Features.Drivers.Commands.UpdateDriverProfile.UpdateDriverProfileCommand(user.DriverId.Value, dto);

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpGet("orders")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Spotless.Application.Dtos.Order.OrderDto>))]
        public async Task<IActionResult> GetDriverOrders()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.DriverId.HasValue)
                return NotFound(new { Message = "Driver profile not found for this user." });

            var query = new Spotless.Application.Features.Orders.Queries.GetDriverOrders.GetDriverOrdersQuery(user.DriverId.Value);

            var orders = await _mediator.Send(query);

            return Ok(orders);
        }

        [HttpGet("available")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<Spotless.Application.Dtos.Order.OrderDto>))]
        public async Task<IActionResult> GetAvailableOrders()
        {
            var query = new Spotless.Application.Features.Orders.Queries.GetAvailableForDrivers.GetAvailableForDriversQuery();

            var orders = await _mediator.Send(query);

            return Ok(orders);
        }

        [HttpPut("status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatus([FromBody] Spotless.Application.Dtos.Driver.DriverStatusUpdateDto dto)
        {
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.DriverId.HasValue)
                return NotFound(new { Message = "Driver profile not found for this user." });

            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest(new { Message = "Status is required." });

            var command = new Spotless.Application.Features.Drivers.Commands.UpdateDriverStatus.UpdateDriverStatusCommand(user.DriverId.Value, dto.Status);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPut("location")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateLocation([FromBody] Spotless.Application.Dtos.LocationDto location)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.DriverId.HasValue)
                return NotFound(new { Message = "Driver profile not found for this user." });

            var command = new Spotless.Application.Features.Drivers.Commands.UpdateDriverLocation.UpdateDriverLocationCommand(user.DriverId.Value, location);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("apply/{orderId:guid}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
        public async Task<IActionResult> ApplyToOrder(Guid orderId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.DriverId.HasValue)
                return NotFound(new { Message = "Driver profile not found for this user." });

            var command = new Spotless.Application.Features.Orders.Commands.ApplyToOrder.ApplyToOrderCommand(orderId, user.DriverId.Value);

            var applicationId = await _mediator.Send(command);

            return CreatedAtAction(nameof(ApplyToOrder), new { id = applicationId }, applicationId);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/applications/{applicationId:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> ApproveApplication(Guid applicationId, [FromBody] Spotless.Application.Dtos.Driver.ApproveDriverRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var adminId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var command = new Spotless.Application.Features.Drivers.Commands.ApproveDriverApplication.ApproveDriverApplicationCommand(
                applicationId,
                request.Password,
                adminId
            );

            var driverId = await _mediator.Send(command);

            return Ok(driverId);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/order-applications/{orderId:guid}")]
        public async Task<IActionResult> GetOrderApplications(Guid orderId)
        {
            var query = new Spotless.Application.Features.Orders.Queries.GetOrderApplications.GetOrderApplicationsQuery(orderId);

            var apps = await _mediator.Send(query);

            return Ok(apps);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/applications/{applicationId:guid}/accept")]
        public async Task<IActionResult> AcceptDriverApplication(Guid applicationId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var adminId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var command = new Spotless.Application.Features.Drivers.Commands.AcceptDriverApplication.AcceptDriverApplicationCommand(applicationId, adminId);

            await _mediator.Send(command);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/assign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignDriver([FromBody] Spotless.Application.Features.Drivers.Commands.AssignDriver.AssignDriverCommand command)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var adminId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            // Append admin id to command before sending
            var commandWithAdmin = command with { AdminId = adminId };

            await _mediator.Send(commandWithAdmin);

            return Ok();
        }
    }
}

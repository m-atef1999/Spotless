using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Features.Drivers.Commands.SubmitDriverApplicationCommand;
using Spotless.Infrastructure.Identity;
using System.Security.Claims;
using Spotless.Application.Interfaces;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DriversController(IMediator mediator, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, ILogger<DriversController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<DriversController> _logger = logger;

        /// <summary>
        /// Submits driver application
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Register([FromBody] DriverApplicationRequest dto)
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

        /// <summary>
        /// Retrieves authenticated driver's profile
        /// </summary>
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
            if (user == null)
                return NotFound(new { Message = "User not found." });

            try
            {
                var driverId = await EnsureDriverProfileAsync(user);
                var query = new Spotless.Application.Features.Drivers.Queries.GetDriverProfile.GetDriverProfileQuery(driverId);
                var dto = await _mediator.Send(query);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Driver profile not found." });
            }
        }

        /// <summary>
        /// Updates authenticated driver's profile
        /// </summary>
        [HttpPut("profile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProfile([FromBody] DriverUpdateRequest dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null)
                return NotFound(new { Message = "User not found." });

            try
            {
                var driverId = await EnsureDriverProfileAsync(user);
                var command = new Spotless.Application.Features.Drivers.Commands.UpdateDriverProfile.UpdateDriverProfileCommand(driverId, dto);
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Driver profile not found." });
            }
        }

        /// <summary>
        /// Updates any driver's profile (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("admin/{driverId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDriverProfileAdmin(Guid driverId, [FromBody] DriverUpdateRequest dto)
        {

            
            var command = new Spotless.Application.Features.Drivers.Commands.UpdateDriverProfile.UpdateDriverProfileCommand(driverId, dto);

            await _mediator.Send(command);

            return NoContent();
        }

        /// <summary>
        /// Revokes driver access (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("admin/{driverId}/revoke")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RevokeDriverAccess(Guid driverId)
        {
            var command = new Spotless.Application.Features.Drivers.Commands.RevokeDriverAccess.RevokeDriverAccessCommand(driverId);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Retrieves orders assigned to authenticated driver
        /// </summary>
        [HttpGet("orders")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Spotless.Application.Dtos.Order.OrderDto>))]
        public async Task<IActionResult> GetDriverOrders()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null)
                return NotFound(new { Message = "User not found." });

            try
            {
                var driverId = await EnsureDriverProfileAsync(user);
                var query = new Spotless.Application.Features.Orders.Queries.GetDriverOrders.GetDriverOrdersQuery(driverId);
                var orders = await _mediator.Send(query);
                return Ok(orders);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Driver profile not found." });
            }
        }

        /// <summary>
        /// Retrieves driver's earnings
        /// </summary>
        [HttpGet("earnings")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Spotless.Application.Dtos.Driver.DriverEarningsDto))]
        public async Task<IActionResult> GetEarnings()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null)
                return NotFound(new { Message = "User not found." });

            try
            {
                var driverId = await EnsureDriverProfileAsync(user);
                var query = new Spotless.Application.Features.Drivers.Queries.GetDriverEarnings.GetDriverEarningsQuery(driverId);
                var earnings = await _mediator.Send(query);
                return Ok(earnings);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Driver profile not found." });
            }
        }

        /// <summary>
        /// Retrieves available orders for drivers to accept
        /// </summary>
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
            if (user == null)
                return NotFound(new { Message = "User not found." });

            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest(new { Message = "Status is required." });

            try
            {
                var driverId = await EnsureDriverProfileAsync(user);
                var command = new Spotless.Application.Features.Drivers.Commands.UpdateDriverStatus.UpdateDriverStatusCommand(driverId, dto.Status);
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Driver profile not found." });
            }
        }

        [HttpPut("location")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateLocation([FromBody] Spotless.Application.Dtos.LocationDto location)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null)
                return NotFound(new { Message = "User not found." });

            try
            {
                var driverId = await EnsureDriverProfileAsync(user);
                var command = new Spotless.Application.Features.Drivers.Commands.UpdateDriverLocation.UpdateDriverLocationCommand(driverId, location);
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Driver profile not found." });
            }
        }

        [HttpPost("apply/{orderId:guid}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
        public async Task<IActionResult> ApplyToOrder(Guid orderId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null)
                return NotFound(new { Message = "User not found." });

            try
            {
                var driverId = await EnsureDriverProfileAsync(user);
                var command = new Spotless.Application.Features.Orders.Commands.ApplyToOrder.ApplyToOrderCommand(orderId, driverId);
                var applicationId = await _mediator.Send(command);
                return CreatedAtAction(nameof(ApplyToOrder), new { id = applicationId }, applicationId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Driver profile not found." });
            }
        }

        // --- Driver Registration Endpoints ---

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/registrations/{applicationId:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        public async Task<IActionResult> ApproveDriverRegistration(Guid applicationId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var adminId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var resolvedApplicationId = await ResolveApplicationId(applicationId);
            
            if (resolvedApplicationId == null)
            {
                return NotFound(new { Message = $"Driver application with ID {applicationId} not found." });
            }

            var command = new Spotless.Application.Features.Drivers.Commands.ApproveDriverApplication.ApproveDriverApplicationCommand(
                resolvedApplicationId.Value,
                adminId
            );

            var driverId = await _mediator.Send(command);

            return Ok(driverId);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/registrations/{applicationId:guid}/reject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RejectDriverRegistration(Guid applicationId, [FromBody] string reason)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var adminId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var resolvedApplicationId = await ResolveApplicationId(applicationId);
            if (resolvedApplicationId == null)
            {
                return NotFound(new { Message = $"Driver application with ID {applicationId} not found." });
            }

            var command = new Spotless.Application.Features.Drivers.Commands.RejectDriverRegistration.RejectDriverRegistrationCommand(
                resolvedApplicationId.Value,
                adminId,
                reason
            );

            await _mediator.Send(command);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/applications")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Spotless.Application.Dtos.Responses.PagedResponse<DriverApplicationDto>))]
        public async Task<IActionResult> GetDriverApplications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] Spotless.Domain.Enums.DriverApplicationStatus? status = null)
        {
            var query = new Spotless.Application.Features.Drivers.Queries.GetDriverApplications.GetDriverApplicationsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Status = status
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        private async Task<Guid?> ResolveApplicationId(Guid inputId)
        {
            // 1. Try to find application directly
            var application = await _unitOfWork.DriverApplications.GetByIdAsync(inputId);
            if (application != null) 
            {
                return application.Id;
            }

            // 2. Try to find as User ID (since Driver.Id == UserId)
            // This is the most reliable link: DriverId (Frontend) -> UserId -> DriverId (which stores ApplicationId)
            var user = await _userManager.FindByIdAsync(inputId.ToString());
            if (user != null)
            {
                if (user.DriverId.HasValue)
                {
                    // Verify this ID exists in DriverApplications
                    var appFromUser = await _unitOfWork.DriverApplications.GetByIdAsync(user.DriverId.Value);
                    if (appFromUser != null)
                    {
                        return appFromUser.Id;
                    }
                }

                // Fallback: Check via CustomerId if DriverId was null or invalid
                if (user.CustomerId.HasValue)
                {
                    application = await _unitOfWork.DriverApplications.GetSingleAsync(da => da.CustomerId == user.CustomerId.Value);
                    if (application != null)
                    {
                        return application.Id;
                    }
                }
            }

            // 3. Fallback: Try to find as Driver entity and match by Email (Case Insensitive)
            var driver = await _unitOfWork.Drivers.GetByIdAsync(inputId);
            if (driver != null)
            {
                // Found a driver. Find the corresponding application via Email
                // Note: GetByEmailAsync might be case sensitive, so we might fail here if cases differ
                var customer = await _unitOfWork.Customers.GetByEmailAsync(driver.Email);
                if (customer != null)
                {
                    application = await _unitOfWork.DriverApplications.GetSingleAsync(da => da.CustomerId == customer.Id);
                    if (application != null) 
                    {
                        return application.Id;
                    }
                }
            }
            
            return null;
        }

        // --- Order Application Endpoints ---

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/order-applications/{orderId:guid}")]
        public async Task<IActionResult> GetOrderApplications(Guid orderId)
        {
            var query = new Spotless.Application.Features.Orders.Queries.GetOrderApplications.GetOrderApplicationsQuery(orderId);

            var apps = await _mediator.Send(query);

            return Ok(apps);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/order-applications/{applicationId:guid}/accept")]
        public async Task<IActionResult> AcceptOrderApplication(Guid applicationId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var adminId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var command = new Spotless.Application.Features.Drivers.Commands.AcceptDriverApplication.AcceptDriverApplicationCommand(applicationId, adminId);

            await _mediator.Send(command);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/order-applications/{applicationId:guid}/reject")]
        public async Task<IActionResult> RejectOrderApplication(Guid applicationId)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var adminId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var command = new Spotless.Application.Features.Orders.Commands.RejectOrderApplication.RejectOrderApplicationCommand(applicationId, adminId);

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

        /// <summary>
        /// Lists all drivers with optional filtering
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Spotless.Application.Dtos.Responses.PagedResponse<DriverDto>))]
        public async Task<IActionResult> GetDrivers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] Spotless.Domain.Enums.DriverStatus? status = null, [FromQuery] string? searchTerm = null)
        {
            var query = new Spotless.Application.Features.Drivers.Queries.ListDriverQuery.ListDriversQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                StatusFilter = status,
                NameSearchTerm = searchTerm
            };

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        private async Task<Guid> EnsureDriverProfileAsync(ApplicationUser user)
        {
            // 1. If DriverId is set, verify it exists
            if (user.DriverId.HasValue)
            {
                var exists = await _unitOfWork.Drivers.GetByIdAsync(user.DriverId.Value);
                if (exists != null) return user.DriverId.Value;
            }

            // 2. Try find by Email
            if (!string.IsNullOrEmpty(user.Email))
            {
                var driver = await _unitOfWork.Drivers.GetByEmailAsync(user.Email);
                if (driver != null)
                {
                    user.DriverId = driver.Id;
                    await _userManager.UpdateAsync(user);
                    return driver.Id;
                }
            }

            // 3. Try find by CustomerId (if linked) and check for Approved Application
            if (user.CustomerId.HasValue)
            {
                var app = await _unitOfWork.DriverApplications.GetSingleAsync(da => da.CustomerId == user.CustomerId.Value);
                if (app != null && app.Status == Spotless.Domain.Enums.DriverApplicationStatus.Approved)
                {
                    // Create Driver!
                    var customer = await _unitOfWork.Customers.GetByIdAsync(user.CustomerId.Value);
                    if (customer != null)
                    {
                        var newDriver = new Spotless.Domain.Entities.Driver(
                            adminId: null,
                            name: customer.Name,
                            email: customer.Email,
                            phone: customer.Phone,
                            vehicleInfo: app.VehicleInfo
                        );
                        newDriver.SetIdentityId(user.Id);
                        newDriver.UpdateStatus(Spotless.Domain.Enums.DriverStatus.Offline);
                        
                        await _unitOfWork.Drivers.AddAsync(newDriver);
                        await _unitOfWork.CommitAsync();

                        user.DriverId = newDriver.Id;
                        await _userManager.UpdateAsync(user);
                        return newDriver.Id;
                    }
                }
            }

            throw new KeyNotFoundException("Driver profile not found and could not be recovered.");
        }

    }
}

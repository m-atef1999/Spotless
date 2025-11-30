using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Admin;
using Spotless.Application.Features.Admins.Queries.GetAdminDashboard;
using Spotless.Application.Interfaces;
using Spotless.Infrastructure.Identity;
using System.Security.Claims;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/admins")]
    [Authorize(Roles = "Admin")]
    public class AdminsController(
        IMediator mediator,
        UserManager<ApplicationUser> userManager,
        IPaginationService paginationService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IPaginationService _paginationService = paginationService;

        
        
        /// <summary>
        /// Lists all administrators with pagination
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Spotless.Application.Dtos.Responses.PagedResponse<Spotless.Application.Dtos.Admin.AdminDto>), 200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListAdmins(
            [FromQuery] string? searchTerm,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            pageNumber ??= _paginationService.GetDefaultPageNumber();
            pageSize = _paginationService.NormalizePageSize(pageSize);

            var query = new Spotless.Application.Features.Admins.Queries.ListAdmins.ListAdminsQuery(searchTerm)
            {
                PageNumber = pageNumber.Value,
                PageSize = pageSize.Value
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new admin user
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAdmin([FromBody] Spotless.Application.Features.Admins.Commands.CreateAdmin.CreateAdminCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { message = "Admin created successfully", userId = result.UserId, email = result.Email });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves admin dashboard with system statistics
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdminDashboardDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDashboard()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out _))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.AdminId.HasValue)
                return Forbid("Admin profile not found for this user.");

            // Use fixed small page size for most used services (top 5)
            var query = new GetAdminDashboardQuery(1, 5);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}

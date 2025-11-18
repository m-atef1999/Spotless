using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Admin;
using Spotless.Application.Features.Admins.Queries.GetAdminDashboard;
using Spotless.Infrastructure.Identity;
using System.Security.Claims;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdminDashboardDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDashboard()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });
            }

            var user = await _userManager.FindByIdAsync(userIdString);

            if (user == null || !user.AdminId.HasValue)
            {
                return Forbid("Admin profile not found for this user.");
            }

            var query = new GetAdminDashboardQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}


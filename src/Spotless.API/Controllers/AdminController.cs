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
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPaginationService _paginationService;

        public AdminController(
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            IPaginationService paginationService)
        {
            _mediator = mediator;
            _userManager = userManager;
            _paginationService = paginationService;
        }

        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdminDashboardDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDashboard(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized(new { Message = "Invalid or missing user ID claim." });

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || !user.AdminId.HasValue)
                return Forbid("Admin profile not found for this user.");

            pageNumber ??= _paginationService.GetDefaultPageNumber();
            pageSize = _paginationService.NormalizePageSize(pageSize);

            var query = new GetAdminDashboardQuery(pageNumber.Value, pageSize.Value);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}

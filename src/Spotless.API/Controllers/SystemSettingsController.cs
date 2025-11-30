using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Settings;
using Spotless.Application.Features.Settings.Commands.UpdateSetting;
using Spotless.Application.Features.Settings.Queries.ListSettings;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/settings")]
    [Authorize(Roles = "Admin")]
    public class SystemSettingsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        
        
        
        /// <summary>
        /// Retrieves system settings with optional category filter (Admin only)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<SystemSettingDto>), 200)]
        public async Task<IActionResult> ListSettings([FromQuery] string? category)
        {
            var query = new ListSettingsQuery(category);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Updates a system setting value (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateSetting(Guid id, [FromBody] UpdateSystemSettingDto dto)
        {
            var command = new UpdateSettingCommand(id, dto);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}

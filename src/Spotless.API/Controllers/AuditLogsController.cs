using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.AuditLog;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Features.AuditLogs.Queries.ListAuditLogs;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/audit-logs")]
    [Authorize(Roles = "Admin")]
    public class AuditLogsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        
        
        /// <summary>
        /// Retrieves audit logs with optional filtering and pagination (Admin only)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<AuditLogDto>), 200)]
        public async Task<IActionResult> ListAuditLogs(
            [FromQuery] Guid? userId,
            [FromQuery] string? eventType,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = new ListAuditLogsQuery(userId, eventType, startDate, endDate, pageNumber, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotless.Application.Dtos.Analytics;
using Spotless.Application.Features.Analytics.Queries.GetAdminDashboard;
using Spotless.Application.Features.Analytics.Queries.GetRevenueReport;

namespace Spotless.API.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    [Authorize(Roles = "Admin")]
    public class AnalyticsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Retrieves admin dashboard metrics (Admin only)
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(AdminDashboardDto), 200)]
        public async Task<IActionResult> GetDashboard()
        {
            var query = new GetAdminDashboardQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves revenue report for a date range (Admin only)
        /// </summary>
        [HttpGet("revenue")]
        [ProducesResponseType(typeof(RevenueReportDto), 200)]
        public async Task<IActionResult> GetRevenueReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var query = new GetRevenueReportQuery(startDate, endDate);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}

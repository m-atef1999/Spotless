using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;
using System.Text.Json;

namespace Spotless.API.Filters
{
    public class AuditActionFilter : IAsyncActionFilter
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AuditActionFilter> _logger;

        public AuditActionFilter(ApplicationDbContext dbContext, ILogger<AuditActionFilter> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Capture action arguments (these are already bound by model binding)
            string data = "";
            try
            {
                if (context.ActionArguments != null && context.ActionArguments.Count > 0)
                {
                    data = JsonSerializer.Serialize(context.ActionArguments);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to serialize action arguments for audit");
            }

            var executedContext = await next();

            try
            {
                var httpContext = context.HttpContext;
                var userIdClaim = httpContext.User?.FindFirst("sub")?.Value;
                Guid? userId = null;
                if (Guid.TryParse(userIdClaim, out var parsed)) userId = parsed;

                var audit = new AuditLog
                {
                    EventType = context.ActionDescriptor?.DisplayName ?? context.HttpContext.Request.Path,
                    UserId = userId,
                    UserName = httpContext.User?.Identity?.Name ?? string.Empty,
                    Data = data ?? string.Empty,
                    IpAddress = httpContext.Connection?.RemoteIpAddress?.ToString() ?? string.Empty,
                    CorrelationId = httpContext.TraceIdentifier ?? string.Empty,
                    OccurredAt = DateTime.UtcNow
                };

                _dbContext.AuditLogs.Add(audit);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write audit log");
            }
        }
    }
}

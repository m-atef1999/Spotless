using MediatR;
using Spotless.Application.Dtos.AuditLog;
using Spotless.Application.Dtos.Responses;

namespace Spotless.Application.Features.AuditLogs.Queries.ListAuditLogs
{
    public record ListAuditLogsQuery(
        Guid? UserId,
        string? EventType,
        DateTime? StartDate,
        DateTime? EndDate,
        int PageNumber,
        int PageSize
    ) : IRequest<PagedResponse<AuditLogDto>>;
}

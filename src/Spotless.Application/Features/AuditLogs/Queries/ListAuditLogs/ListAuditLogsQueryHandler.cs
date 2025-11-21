using MediatR;
using Spotless.Application.Dtos.AuditLog;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.AuditLogs.Queries.ListAuditLogs
{
    public class ListAuditLogsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<ListAuditLogsQuery, PagedResponse<AuditLogDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<PagedResponse<AuditLogDto>> Handle(ListAuditLogsQuery request, CancellationToken cancellationToken)
        {
            // Build filter predicate
            var filter = BuildFilter(request);

            // Get total count
            var totalCount = await _unitOfWork.AuditLogs.CountAsync(filter);

            // Get paged data
            var skip = (request.PageNumber - 1) * request.PageSize;
            var auditLogs = await _unitOfWork.AuditLogs.GetPagedAsync(
                filter,
                skip,
                request.PageSize,
                orderBy: q => q.OrderByDescending(a => a.OccurredAt)
            );

            // Map to DTOs
            var dtos = auditLogs.Select(a => new AuditLogDto
            {
                Id = a.Id,
                EventType = a.EventType,
                UserId = a.UserId,
                UserName = a.UserName,
                Data = a.Data,
                IpAddress = a.IpAddress,
                CorrelationId = a.CorrelationId,
                OccurredAt = a.OccurredAt
            }).ToList();

            return new PagedResponse<AuditLogDto>
            {
                Data = dtos,
                TotalRecords = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        private static System.Linq.Expressions.Expression<Func<Domain.Entities.AuditLog, bool>> BuildFilter(ListAuditLogsQuery request)
        {
            return a =>
                (!request.UserId.HasValue || a.UserId == request.UserId) &&
                (string.IsNullOrEmpty(request.EventType) || a.EventType.Contains(request.EventType)) &&
                (!request.StartDate.HasValue || a.OccurredAt >= request.StartDate) &&
                (!request.EndDate.HasValue || a.OccurredAt <= request.EndDate);
        }
    }
}

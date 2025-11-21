using MediatR;
using Spotless.Application.Dtos.Analytics;

namespace Spotless.Application.Features.Analytics.Queries.GetRevenueReport
{
    public record GetRevenueReportQuery(DateTime StartDate, DateTime EndDate) : IRequest<RevenueReportDto>;
}

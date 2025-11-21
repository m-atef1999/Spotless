using MediatR;
using Spotless.Application.Dtos.Analytics;

namespace Spotless.Application.Features.Analytics.Queries.GetAdminDashboard
{
    public record GetAdminDashboardQuery : IRequest<AdminDashboardDto>;
}

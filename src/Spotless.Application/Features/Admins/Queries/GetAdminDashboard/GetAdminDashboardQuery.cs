using Spotless.Application.Dtos.Admin;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Admins.Queries.GetAdminDashboard
{
    public record GetAdminDashboardQuery(
        int PageNumber = 1,
        int PageSize = 10
    ) : IQuery<AdminDashboardDto>;
}

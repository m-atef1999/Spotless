using MediatR;
using Spotless.Application.Dtos.Admin;

namespace Spotless.Application.Features.Admins.Queries.GetAdminDashboard
{
    public record GetAdminDashboardQuery() : IRequest<AdminDashboardDto>;
}


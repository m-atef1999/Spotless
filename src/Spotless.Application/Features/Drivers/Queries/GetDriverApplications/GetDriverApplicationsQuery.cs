using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Dtos.Responses;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Queries.GetDriverApplications
{
    public class GetDriverApplicationsQuery : IRequest<PagedResponse<DriverApplicationDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DriverApplicationStatus? Status { get; set; }
    }
}

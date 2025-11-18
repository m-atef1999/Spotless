using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Dtos.Responses;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Queries.ListDriverQuery
{

    public class ListDriversQuery : IRequest<PagedResponse<DriverDto>>
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;


        public int Skip => (PageNumber - 1) * PageSize;

        public DriverStatus? StatusFilter { get; init; }
        public string? NameSearchTerm { get; init; }
    }
}
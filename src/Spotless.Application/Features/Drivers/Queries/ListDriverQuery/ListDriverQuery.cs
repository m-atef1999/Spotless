using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Dtos.Requests;
using Spotless.Application.Dtos.Responses;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Queries
{
    public record ListDriversQuery(
        DriverStatus? StatusFilter,
        string? NameSearchTerm
    ) : PaginationBaseRequest, IRequest<PagedResponse<DriverProfileDto>>;
}
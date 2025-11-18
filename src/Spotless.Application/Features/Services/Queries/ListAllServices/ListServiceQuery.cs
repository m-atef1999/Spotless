using Spotless.Application.Dtos.Requests;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Services.Queries.ListAllServices
{

    public record ListServicesQuery(
        string? NameSearchTerm

    ) : PaginationBaseRequest, IQuery<PagedResponse<ServiceDto>>;
}
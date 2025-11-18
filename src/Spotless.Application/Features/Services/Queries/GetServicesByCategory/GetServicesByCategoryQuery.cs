using Spotless.Application.Dtos.Requests;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Services.Queries.GetServicesByCategory
{

    public record ListServicesByCategoryQuery(
        Guid CategoryId,
        string? NameSearchTerm
    ) : PaginationBaseRequest, IQuery<PagedResponse<ServiceDto>>;
}
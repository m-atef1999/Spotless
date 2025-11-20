using MediatR;
using Spotless.Application.Dtos.Requests;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Dtos.Service;

namespace Spotless.Application.Features.Services.Queries.ListAllServices
{
    public record ListServicesQuery(
        string? NameSearchTerm
    ) : PaginationBaseRequest, IRequest<PagedResponse<ServiceDto>>;
}

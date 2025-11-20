using MediatR;
using Spotless.Application.Dtos.Admin;
using Spotless.Application.Dtos.Requests;
using Spotless.Application.Dtos.Responses;

namespace Spotless.Application.Features.Admins.Queries.ListAdmins
{
    public record ListAdminsQuery(string? SearchTerm) : PaginationBaseRequest, IRequest<PagedResponse<AdminDto>>;
}

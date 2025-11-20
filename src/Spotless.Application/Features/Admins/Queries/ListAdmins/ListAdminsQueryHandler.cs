using MediatR;
using Spotless.Application.Dtos.Admin;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Services;

namespace Spotless.Application.Features.Admins.Queries.ListAdmins
{
    public class ListAdminsQueryHandler(CachedAdminService cachedAdminService) 
        : IRequestHandler<ListAdminsQuery, PagedResponse<AdminDto>>
    {
        private readonly CachedAdminService _cachedAdminService = cachedAdminService;

        public async Task<PagedResponse<AdminDto>> Handle(ListAdminsQuery request, CancellationToken cancellationToken)
        {
            var cachedAdmins = await _cachedAdminService.GetAllAdminsAsync();

            // Apply search filter if provided
            var filtered = string.IsNullOrWhiteSpace(request.SearchTerm)
                ? cachedAdmins
                : cachedAdmins.Where(a => 
                    a.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    a.Email.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));

            // Apply pagination
            var pagedAdmins = filtered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResponse<AdminDto>(
                pagedAdmins,
                filtered.Count(),
                request.PageNumber,
                request.PageSize
            );
        }
    }
}

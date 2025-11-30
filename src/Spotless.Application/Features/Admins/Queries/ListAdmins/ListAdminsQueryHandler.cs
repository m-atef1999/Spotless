using MediatR;
using Spotless.Application.Dtos.Admin;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Services;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Admins.Queries.ListAdmins
{
    public class ListAdminsQueryHandler(IAdminRepository adminRepository) 
        : IRequestHandler<ListAdminsQuery, PagedResponse<AdminDto>>
    {
        private readonly IAdminRepository _adminRepository = adminRepository;

        public async Task<PagedResponse<AdminDto>> Handle(ListAdminsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;
            var searchTerm = request.SearchTerm?.Trim().ToLower();

            // Use BaseRepository's GetPagedAsync which handles pagination efficiently
            // We need to provide a filter expression if search term is present
            
            System.Linq.Expressions.Expression<Func<Spotless.Domain.Entities.Admin, bool>> filter = a => true;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filter = a => a.Name.ToLower().Contains(searchTerm) || 
                              a.Email.ToLower().Contains(searchTerm);
            }

            var totalCount = await _adminRepository.CountAsync(filter);

            var pagedAdmins = await _adminRepository.GetPagedAsync(
                filter,
                (pageNumber - 1) * pageSize,
                pageSize,
                null,
                q => q.OrderBy(a => a.Name)
            );

            var adminDtos = pagedAdmins.Select(a => new AdminDto
            {
                Id = a.Id,
                Name = a.Name,
                Email = a.Email,
                AdminRole = a.AdminRole.ToString()
            }).ToList();

            return new PagedResponse<AdminDto>(
                adminDtos,
                totalCount,
                pageNumber,
                pageSize
            );
        }
    }
}

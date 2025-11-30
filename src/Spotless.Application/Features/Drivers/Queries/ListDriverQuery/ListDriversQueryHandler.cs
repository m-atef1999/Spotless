using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;
using Spotless.Application.Services;

namespace Spotless.Application.Features.Drivers.Queries.ListDriverQuery
{
    public class ListDriversQueryHandler(IDriverRepository driverRepository) 
        : IRequestHandler<ListDriversQuery, PagedResponse<DriverDto>>
    {
        private readonly IDriverRepository _driverRepository = driverRepository;

        public async Task<PagedResponse<DriverDto>> Handle(ListDriversQuery request, CancellationToken cancellationToken)
        {
            System.Linq.Expressions.Expression<Func<Spotless.Domain.Entities.Driver, bool>> filter = d => true;

            if (request.StatusFilter.HasValue)
            {
                var status = request.StatusFilter.Value;
                filter = d => d.Status == status;
            }

            if (!string.IsNullOrEmpty(request.NameSearchTerm))
            {
                var searchTerm = request.NameSearchTerm.Trim().ToLowerInvariant();
                // Combine filters
                var oldFilter = filter;
                filter = d => oldFilter.Compile()(d) && d.Name.ToLower().Contains(searchTerm);
                // Note: The above combination forces client-side evaluation if not careful. 
                // Better to build the expression properly or use separate checks if the repo supports it.
                // But since we are replacing the whole thing, let's do it right.
            }
            
            // Re-implementing filter logic correctly for EF Core
            filter = d => (request.StatusFilter == null || d.Status == request.StatusFilter.Value) &&
                          (string.IsNullOrEmpty(request.NameSearchTerm) || d.Name.ToLower().Contains(request.NameSearchTerm.ToLower()));

            var totalCount = await _driverRepository.CountAsync(filter);

            var drivers = await _driverRepository.GetPagedAsync(
                filter,
                (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                null,
                q => q.OrderBy(d => d.Name)
            );

            var driverDtos = drivers.Select(d => new DriverDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                Phone = d.Phone,
                VehicleInfo = d.VehicleInfo,
                Status = d.Status.ToString()
            }).ToList();

            return new PagedResponse<DriverDto>(
                driverDtos,
                totalCount,
                request.PageNumber,
                request.PageSize
            );
        }
    }
}

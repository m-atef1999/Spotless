using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Services;

namespace Spotless.Application.Features.Drivers.Queries.ListDriverQuery
{
    public class ListDriversQueryHandler(CachedDriverService cachedDriverService) 
        : IRequestHandler<ListDriversQuery, PagedResponse<DriverDto>>
    {
        private readonly CachedDriverService _cachedDriverService = cachedDriverService;

        public async Task<PagedResponse<DriverDto>> Handle(ListDriversQuery request, CancellationToken cancellationToken)
        {
            var cachedDrivers = await _cachedDriverService.GetAllDriversAsync();

            // Apply filters
            var filtered = cachedDrivers.AsEnumerable();

            if (request.StatusFilter.HasValue)
            {
                filtered = filtered.Where(d => d.Status == request.StatusFilter.Value.ToString());
            }

            if (!string.IsNullOrEmpty(request.NameSearchTerm))
            {
                var searchTerm = request.NameSearchTerm.Trim().ToLowerInvariant();
                filtered = filtered.Where(d => d.Name.ToLowerInvariant().Contains(searchTerm));
            }

            // Apply pagination
            var pagedDrivers = filtered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResponse<DriverDto>(
                pagedDrivers,
                filtered.Count(),
                request.PageNumber,
                request.PageSize
            );
        }
    }
}

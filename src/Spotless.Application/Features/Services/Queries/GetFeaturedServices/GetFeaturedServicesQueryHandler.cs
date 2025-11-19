using MediatR;
using Spotless.Application.Dtos.Service;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Application.Services;

namespace Spotless.Application.Features.Services.Queries.GetFeaturedServices
{
    public class GetFeaturedServicesQueryHandler : IRequestHandler<GetFeaturedServicesQuery, IReadOnlyList<ServiceDto>>
    {
        private readonly CachedServiceService _cachedServiceService;

        public GetFeaturedServicesQueryHandler(CachedServiceService cachedServiceService)
        {
            _cachedServiceService = cachedServiceService;
        }

        public async Task<IReadOnlyList<ServiceDto>> Handle(GetFeaturedServicesQuery request, CancellationToken cancellationToken)
        {
            var featuredServices = await _cachedServiceService.GetFeaturedServicesAsync();
            return featuredServices.Take(request.Count).ToList();
        }
    }
}
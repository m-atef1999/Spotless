using Spotless.Application.Dtos.Service;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;

namespace Spotless.Application.Services
{
    public class CachedServiceService(IServiceRepository serviceRepository, ICachingService cachingService, IServiceMapper serviceMapper)
    {
        private readonly IServiceRepository _serviceRepository = serviceRepository;
        private readonly ICachingService _cachingService = cachingService;
        private readonly IServiceMapper _serviceMapper = serviceMapper;
        private const string SERVICES_CACHE_KEY = "services:all";
        private const string FEATURED_SERVICES_CACHE_KEY = "services:featured";

        public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
        {
            var cached = await _cachingService.GetAsync<IEnumerable<ServiceDto>>(SERVICES_CACHE_KEY);
            if (cached != null) return cached;

            var services = await _serviceRepository.GetAllAsync();
            var serviceDtos = _serviceMapper.MapToDto(services);

            await _cachingService.SetAsync(SERVICES_CACHE_KEY, serviceDtos, TimeSpan.FromHours(2));
            return serviceDtos;
        }

        public async Task<IEnumerable<ServiceDto>> GetFeaturedServicesAsync()
        {
            var cached = await _cachingService.GetAsync<IEnumerable<ServiceDto>>(FEATURED_SERVICES_CACHE_KEY);
            if (cached != null) return cached;

            var services = await _serviceRepository.GetFeaturedServicesAsync();
            var serviceDtos = _serviceMapper.MapToDto(services);

            await _cachingService.SetAsync(FEATURED_SERVICES_CACHE_KEY, serviceDtos, TimeSpan.FromHours(4));
            return serviceDtos;
        }

        public async Task InvalidateServiceCacheAsync()
        {
            await _cachingService.RemoveAsync(SERVICES_CACHE_KEY);
            await _cachingService.RemoveAsync(FEATURED_SERVICES_CACHE_KEY);
        }
    }
}

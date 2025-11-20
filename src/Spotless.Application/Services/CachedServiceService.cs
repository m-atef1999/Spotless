using Spotless.Application.Dtos.Service;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Services
{
    public class CachedServiceService(IServiceRepository serviceRepository, ICachingService cachingService)
    {
        private readonly IServiceRepository _serviceRepository = serviceRepository;
        private readonly ICachingService _cachingService = cachingService;
        private const string SERVICES_CACHE_KEY = "services:all";
        private const string FEATURED_SERVICES_CACHE_KEY = "services:featured";

        public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
        {
            var cached = await _cachingService.GetAsync<IEnumerable<ServiceDto>>(SERVICES_CACHE_KEY);
            if (cached != null) return cached;

            var services = await _serviceRepository.GetAllAsync();
            var serviceDtos = services.Select(s => new ServiceDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                BasePrice = s.BasePrice.Amount,
                CategoryId = s.CategoryId,
                EstimatedDurationHours = s.EstimatedDurationHours,
                IsActive = s.IsActive,
                IsFeatured = s.IsFeatured
            });

            await _cachingService.SetAsync(SERVICES_CACHE_KEY, serviceDtos, TimeSpan.FromHours(2));
            return serviceDtos;
        }

        public async Task<IEnumerable<ServiceDto>> GetFeaturedServicesAsync()
        {
            var cached = await _cachingService.GetAsync<IEnumerable<ServiceDto>>(FEATURED_SERVICES_CACHE_KEY);
            if (cached != null) return cached;

            var services = await _serviceRepository.GetFeaturedServicesAsync();
            var serviceDtos = services.Select(s => new ServiceDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                BasePrice = s.BasePrice.Amount,
                CategoryId = s.CategoryId,
                EstimatedDurationHours = s.EstimatedDurationHours,
                IsActive = s.IsActive,
                IsFeatured = s.IsFeatured
            });

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
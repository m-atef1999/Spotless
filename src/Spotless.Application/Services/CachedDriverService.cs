using Spotless.Application.Dtos.Driver;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Services
{
    public class CachedDriverService(IDriverRepository driverRepository, ICachingService cachingService)
    {
        private readonly IDriverRepository _driverRepository = driverRepository;
        private readonly ICachingService _cachingService = cachingService;
        private const string DRIVERS_CACHE_KEY = "drivers:all";

        public async Task<IEnumerable<DriverDto>> GetAllDriversAsync()
        {
            var cached = await _cachingService.GetAsync<IEnumerable<DriverDto>>(DRIVERS_CACHE_KEY);
            if (cached != null) return cached;

            var drivers = await _driverRepository.GetAllAsync();
            var driverDtos = drivers.Select(d => new DriverDto
            {
                Id = d.Id,
                Name = d.Name,
                Email = d.Email,
                Phone = d.Phone,
                VehicleInfo = d.VehicleInfo,
                Status = d.Status.ToString()
            });

            await _cachingService.SetAsync(DRIVERS_CACHE_KEY, driverDtos, TimeSpan.FromHours(2));
            return driverDtos;
        }

        public async Task InvalidateDriverCacheAsync()
        {
            await _cachingService.RemoveAsync(DRIVERS_CACHE_KEY);
        }
    }
}

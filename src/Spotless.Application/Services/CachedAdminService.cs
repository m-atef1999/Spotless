using Spotless.Application.Dtos.Admin;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Services
{
    public class CachedAdminService(IAdminRepository adminRepository, ICachingService cachingService)
    {
        private readonly IAdminRepository _adminRepository = adminRepository;
        private readonly ICachingService _cachingService = cachingService;
        private const string ADMINS_CACHE_KEY = "admins:all";

        public async Task<IEnumerable<AdminDto>> GetAllAdminsAsync()
        {
            var cached = await _cachingService.GetAsync<IEnumerable<AdminDto>>(ADMINS_CACHE_KEY);
            if (cached != null) return cached;

            var admins = await _adminRepository.GetAllAsync();
            var adminDtos = admins.Select(a => new AdminDto
            {
                Id = a.Id,
                Name = a.Name,
                Email = a.Email,
                AdminRole = a.AdminRole.ToString()
            });

            await _cachingService.SetAsync(ADMINS_CACHE_KEY, adminDtos, TimeSpan.FromHours(2));
            return adminDtos;
        }

        public async Task InvalidateAdminCacheAsync()
        {
            await _cachingService.RemoveAsync(ADMINS_CACHE_KEY);
        }
    }
}

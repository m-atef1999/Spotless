using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;

namespace Spotless.Application.Services
{
    public class CachedCategoryService(ICategoryRepository categoryRepository, ICachingService cachingService)
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly ICachingService _cachingService = cachingService;
        private const string CATEGORIES_CACHE_KEY = "categories:all";

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var cached = await _cachingService.GetAsync<IEnumerable<Category>>(CATEGORIES_CACHE_KEY);
            if (cached != null) return cached;

            // Use GetAllWithServicesAsync to include Services collection for accurate ServiceCount
            var categories = await _categoryRepository.GetAllWithServicesAsync();
            await _cachingService.SetAsync(CATEGORIES_CACHE_KEY, categories, TimeSpan.FromHours(6));
            return categories;
        }

        public async Task InvalidateCategoryCacheAsync()
        {
            await _cachingService.RemoveAsync(CATEGORIES_CACHE_KEY);
        }
    }
}

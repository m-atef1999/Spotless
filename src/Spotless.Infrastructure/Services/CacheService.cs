using Microsoft.Extensions.Logging;
using Spotless.Application.Interfaces;
using System.Text.Json;

namespace Spotless.Infrastructure.Services
{
    public class CacheService(ILogger<CacheService> logger) : ICacheService
    {
        private readonly ILogger<CacheService> _logger = logger;
        private readonly Dictionary<string, (object Value, DateTime Expiration)> _cache = [];

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                if (cached.Expiration > DateTime.UtcNow)
                {
                    _logger.LogInformation("Cache hit for key: {Key}", key);
                    return cached.Value as T;
                }
                _cache.Remove(key);
            }
            
            _logger.LogInformation("Cache miss for key: {Key}", key);
            return await Task.FromResult<T?>(null);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            var exp = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : DateTime.UtcNow.AddHours(1);
            _cache[key] = (value, exp);
            _logger.LogInformation("Cache set for key: {Key}, expires: {Expiration}", key, exp);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            _logger.LogInformation("Cache removed for key: {Key}", key);
            await Task.CompletedTask;
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            var keysToRemove = _cache.Keys.Where(k => k.Contains(pattern)).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
            _logger.LogInformation("Cache removed for pattern: {Pattern}, {Count} keys removed", pattern, keysToRemove.Count);
            await Task.CompletedTask;
        }
    }
}

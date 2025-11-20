using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Spotless.Application.Interfaces;
using System.Text.Json;

namespace Spotless.Infrastructure.Services
{
    public class DistributedCachingService(IDistributedCache distributedCache, ILogger<DistributedCachingService> logger) : ICachingService
    {
        private readonly IDistributedCache _distributedCache = distributedCache;
        private readonly ILogger<DistributedCachingService> _logger = logger;

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                var cachedValue = await _distributedCache.GetStringAsync(key);
                if (cachedValue == null)
                {
                    _logger.LogDebug("Cache miss for key: {Key}", key);
                    return null;
                }

                _logger.LogDebug("Cache hit for key: {Key}", key);
                return JsonSerializer.Deserialize<T>(cachedValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving from cache for key: {Key}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value);
                var options = new DistributedCacheEntryOptions();
                
                if (expiration.HasValue)
                    options.SetAbsoluteExpiration(expiration.Value);
                else
                    options.SetAbsoluteExpiration(TimeSpan.FromHours(1));

                await _distributedCache.SetStringAsync(key, serializedValue, options);
                _logger.LogDebug("Cache set for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key);
                _logger.LogDebug("Cache removed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            _logger.LogWarning("RemoveByPatternAsync not supported by IDistributedCache. Pattern: {Pattern}", pattern);
            await Task.CompletedTask;
        }
    }
}

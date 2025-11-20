using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Spotless.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace Spotless.Infrastructure.Services
{
    /// <summary>
    /// Redis-backed distributed lock service for safe concurrent access to shared resources.
    /// </summary>
    public class RedisDistributedLockService(IDistributedCache cache, ILogger<RedisDistributedLockService> logger) : IDistributedLockService
    {
        private readonly IDistributedCache _cache = cache;
        private readonly ILogger<RedisDistributedLockService> _logger = logger;

        public async Task<string?> AcquireLockAsync(string lockKey, TimeSpan timeout = default)
        {
            if (string.IsNullOrWhiteSpace(lockKey))
                throw new ArgumentException("Lock key cannot be empty.", nameof(lockKey));

            if (timeout == default)
                timeout = TimeSpan.FromSeconds(30);

            var lockToken = Guid.NewGuid().ToString();
            var redisKey = $"lock:{lockKey}";

            try
            {
                // Try to set the lock only if it doesn't exist
                var existingValue = await _cache.GetAsync(redisKey);
                if (existingValue != null)
                {
                    _logger.LogWarning("Failed to acquire lock {LockKey}: already locked", lockKey);
                    return null;
                }

                // Set the lock with the token and timeout
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timeout
                };
                await _cache.SetStringAsync(redisKey, lockToken, options);

                _logger.LogInformation("Lock acquired: {LockKey} with token {Token}", lockKey, lockToken);
                return lockToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error acquiring lock {LockKey}", lockKey);
                return null;
            }
        }

        public async Task ReleaseLockAsync(string lockKey, string lockToken)
        {
            if (string.IsNullOrWhiteSpace(lockKey))
                throw new ArgumentException("Lock key cannot be empty.", nameof(lockKey));

            var redisKey = $"lock:{lockKey}";

            try
            {
                // Verify the lock token matches before releasing (prevent accidental release by other holders)
                var storedToken = await _cache.GetStringAsync(redisKey);
                if (storedToken == lockToken)
                {
                    await _cache.RemoveAsync(redisKey);
                    _logger.LogInformation("Lock released: {LockKey}", lockKey);
                }
                else
                {
                    _logger.LogWarning("Attempted to release lock {LockKey} with mismatched token", lockKey);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing lock {LockKey}", lockKey);
            }
        }

        public async Task<T> ExecuteWithLockAsync<T>(string lockKey, Func<Task<T>> action, TimeSpan? timeout = null)
        {
            if (string.IsNullOrWhiteSpace(lockKey))
                throw new ArgumentException("Lock key cannot be empty.", nameof(lockKey));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var lockTimeout = timeout ?? TimeSpan.FromSeconds(30);
            var lockToken = await AcquireLockAsync(lockKey, lockTimeout) ?? throw new InvalidOperationException($"Failed to acquire lock for key: {lockKey}");
            try
            {
                return await action();
            }
            finally
            {
                await ReleaseLockAsync(lockKey, lockToken);
            }
        }
    }
}

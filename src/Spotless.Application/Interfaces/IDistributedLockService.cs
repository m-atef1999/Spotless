using System;
using System.Threading.Tasks;

namespace Spotless.Application.Interfaces
{
    /// <summary>
    /// Abstraction for distributed locks using Redis or another provider.
    /// Used to ensure safe concurrent access to shared resources (e.g., time-slot booking).
    /// </summary>
    public interface IDistributedLockService
    {
        /// <summary>
        /// Acquires a lock with the given key. Returns a unique lock token if successful, or null if unable to acquire.
        /// </summary>
        Task<string?> AcquireLockAsync(string lockKey, TimeSpan timeout = default);

        /// <summary>
        /// Releases a lock by key and token.
        /// </summary>
        Task ReleaseLockAsync(string lockKey, string lockToken);

        /// <summary>
        /// Executes an action within a distributed lock, automatically acquiring and releasing.
        /// </summary>
        Task<T> ExecuteWithLockAsync<T>(string lockKey, Func<Task<T>> action, TimeSpan? timeout = null);
    }
}

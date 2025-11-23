using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using System.Collections.Concurrent;

namespace Spotless.API.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly int _maxRequests;
        private readonly TimeSpan _timeWindow;
        private readonly ConcurrentDictionary<string, List<DateTime>> _requestHistory;
        
        // AI specific limits
        private readonly int _aiMaxRequests = 15;
        private readonly TimeSpan _aiTimeWindow = TimeSpan.FromMinutes(1);
        private readonly ConcurrentDictionary<string, List<DateTime>> _aiRequestHistory;

        public RateLimitingMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingMiddleware> logger,
            IOptions<SecuritySettings> securitySettings)
        {
            _next = next;
            _logger = logger;

            var rateLimitSettings = securitySettings.Value.RateLimit;
            _maxRequests = rateLimitSettings.MaxRequests;
            _timeWindow = TimeSpan.FromMinutes(rateLimitSettings.TimeWindowMinutes);
            _requestHistory = new ConcurrentDictionary<string, List<DateTime>>();
            _aiRequestHistory = new ConcurrentDictionary<string, List<DateTime>>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTime.UtcNow;
            var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

            if (path.StartsWith("/api/ai"))
            {
                if (await CheckRateLimitAsync(context, clientIp, now, _aiRequestHistory, _aiMaxRequests, _aiTimeWindow, "AI"))
                {
                    return;
                }
            }
            else
            {
                if (await CheckRateLimitAsync(context, clientIp, now, _requestHistory, _maxRequests, _timeWindow, "Global"))
                {
                    return;
                }
            }

            await _next(context);
        }

        private async Task<bool> CheckRateLimitAsync(
            HttpContext context, 
            string clientIp, 
            DateTime now, 
            ConcurrentDictionary<string, List<DateTime>> historyDict, 
            int maxRequests, 
            TimeSpan timeWindow,
            string limitType)
        {
            CleanupOldEntries(historyDict, clientIp, now, timeWindow);

            var history = historyDict.GetOrAdd(clientIp, _ => []);
            bool rateLimitExceeded = false;

            lock (history)
            {
                if (history.Count >= maxRequests)
                {
                    rateLimitExceeded = true;
                }
                else
                {
                    history.Add(now);
                }
            }

            if (rateLimitExceeded)
            {
                _logger.LogWarning(
                    "{LimitType} Rate limit exceeded for IP {ClientIp}. {Count} requests in the last {WindowMinutes} minutes.",
                    limitType,
                    clientIp,
                    history.Count,
                    timeWindow.TotalMinutes);

                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers["Retry-After"] = timeWindow.TotalSeconds.ToString();
                await context.Response.WriteAsync($"{limitType} rate limit exceeded. Please try again later.");
                return true;
            }

            return false;
        }

        private void CleanupOldEntries(ConcurrentDictionary<string, List<DateTime>> historyDict, string clientIp, DateTime now, TimeSpan timeWindow)
        {
            if (historyDict.TryGetValue(clientIp, out var history))
            {
                lock (history)
                {
                    history.RemoveAll(t => now - t > timeWindow);
                    if (history.Count == 0)
                    {
                        historyDict.TryRemove(clientIp, out _);
                    }
                }
            }
        }
    }
}


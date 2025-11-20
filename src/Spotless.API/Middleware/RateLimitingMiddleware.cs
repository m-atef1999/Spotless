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
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTime.UtcNow;

            CleanupOldEntries(clientIp, now);

            var history = _requestHistory.GetOrAdd(clientIp, _ => []);


            bool rateLimitExceeded = false;

            lock (history)
            {

                history.RemoveAll(t => now - t > _timeWindow);


                if (history.Count >= _maxRequests)
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
                    "Rate limit exceeded for IP {ClientIp}. {Count} requests in the last {WindowMinutes} minutes.",
                    clientIp,
                    history.Count,
                    _timeWindow.TotalMinutes);

                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers["Retry-After"] = _timeWindow.TotalSeconds.ToString();


                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");

                return;
            }

            await _next(context);
        }

        private void CleanupOldEntries(string clientIp, DateTime now)
        {
            if (_requestHistory.TryGetValue(clientIp, out var history))
            {
                lock (history)
                {
                    history.RemoveAll(t => now - t > _timeWindow);


                    if (history.Count == 0)
                    {
                        _requestHistory.TryRemove(clientIp, out _);
                    }
                }
            }
        }
    }
}


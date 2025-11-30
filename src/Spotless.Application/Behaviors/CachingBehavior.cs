using MediatR;
using Microsoft.Extensions.Logging;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Behaviors
{
    public class CachingBehavior<TRequest, TResponse>(ICachingService cachingService, ILogger<CachingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICachingService _cachingService = cachingService;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger = logger;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            if (request is ICacheInvalidator cacheInvalidator)
            {
                foreach (var key in cacheInvalidator.CacheKeys)
                {
                    _logger.LogInformation("Invalidating cache key: {CacheKey}", key);
                    await _cachingService.RemoveAsync(key);
                }
            }

            return response;
        }
    }
}

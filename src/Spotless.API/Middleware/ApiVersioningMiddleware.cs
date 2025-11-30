using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;

namespace Spotless.API.Middleware
{
    public class ApiVersioningMiddleware(
        RequestDelegate next,
        ILogger<ApiVersioningMiddleware> logger,
        IOptions<ApiVersioningSettings> settings)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ApiVersioningMiddleware> _logger = logger;
        private readonly ApiVersioningSettings _settings = settings.Value;

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var path = request.Path.Value ?? string.Empty;

            
            
            // Check if request is to API endpoints
            if (path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            {
                
                
                // Extract version from path (e.g., /api/v1/...)
                var pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                
                if (pathSegments.Length >= 2 && pathSegments[0].Equals("api", StringComparison.OrdinalIgnoreCase))
                {
                    var versionSegment = pathSegments[1];
                    
                    // Check if version is specified
                    if (versionSegment.StartsWith("v", StringComparison.OrdinalIgnoreCase))
                    {
                        var requestedVersion = versionSegment.ToLowerInvariant();
                        
                        // Validate version
                        if (!_settings.SupportedVersions.Contains(requestedVersion))
                        {
                            _logger.LogWarning(
                                "Unsupported API version requested: {Version}. Supported versions: {SupportedVersions}",
                                requestedVersion,
                                string.Join(", ", _settings.SupportedVersions));

                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync($"{{\"error\":\"Unsupported API version '{requestedVersion}'. Supported versions: {string.Join(", ", _settings.SupportedVersions)}\"}}");
                            return;
                        }

                        // Store version in context for use in controllers
                        context.Items["ApiVersion"] = requestedVersion;
                    }
                    else
                    {
                        // No version specified, use default
                        if (_settings.RequireVersionInPath)
                        {
                            _logger.LogWarning("API version is required in path but not provided.");
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync($"{{\"error\":\"API version is required in the path. Use format: /api/v1/...\"}}");
                            return;
                        }
                        
                        context.Items["ApiVersion"] = _settings.DefaultVersion;
                    }
                }
                else
                {
                    // Default version for API requests
                    context.Items["ApiVersion"] = _settings.DefaultVersion;
                }
            }

            await _next(context);
        }
    }
}


namespace Spotless.API.Middleware
{

    public class HttpsEnforcementMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpsEnforcementMiddleware> _logger;
        private readonly bool _enforceHttps;

        public HttpsEnforcementMiddleware(
            RequestDelegate next,
            ILogger<HttpsEnforcementMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _enforceHttps = !environment.IsDevelopment();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip HTTPS enforcement in development
            if (_enforceHttps && !context.Request.IsHttps)
            {
                _logger.LogWarning(
                    "HTTPS enforcement: Blocked HTTP request from {RemoteIpAddress} to {Path}",
                    context.Connection.RemoteIpAddress,
                    context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("HTTPS is required. Please use HTTPS to access this API.");
                return;
            }

            await _next(context);
        }
    }
}


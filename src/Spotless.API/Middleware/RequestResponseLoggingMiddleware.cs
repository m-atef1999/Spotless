using System.Diagnostics;
using System.Text;

namespace Spotless.API.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            // Log request
            await LogRequestAsync(context, requestId);

            // Capture response body
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                // Log response
                await LogResponseAsync(context, requestId, stopwatch.ElapsedMilliseconds);

                // Copy response body back to original stream
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task LogRequestAsync(HttpContext context, string requestId)
        {
            context.Items["RequestId"] = requestId;

            var request = context.Request;
            var requestBody = await ReadRequestBodyAsync(request);

            _logger.LogInformation(
                "Request {RequestId}: {Method} {Path} | Query: {QueryString} | Body: {RequestBody} | IP: {RemoteIp}",
                requestId,
                request.Method,
                request.Path,
                request.QueryString,
                requestBody,
                context.Connection.RemoteIpAddress);

            // Restore request body for actual processing
            request.Body.Position = 0;
        }

        private async Task LogResponseAsync(HttpContext context, string requestId, long elapsedMilliseconds)
        {
            var response = context.Response;
            var responseBody = await ReadResponseBodyAsync(response);

            _logger.LogInformation(
                "Response {RequestId}: {StatusCode} | Elapsed: {ElapsedMs}ms | Body: {ResponseBody}",
                requestId,
                response.StatusCode,
                elapsedMilliseconds,
                responseBody);
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();
            }

            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            // Truncate long bodies for logging
            return body.Length > 1000 ? body[..1000] + "..." : body;
        }

        private static async Task<string> ReadResponseBodyAsync(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            // Truncate long bodies for logging
            return body.Length > 1000 ? body[..1000] + "..." : body;
        }
    }
}


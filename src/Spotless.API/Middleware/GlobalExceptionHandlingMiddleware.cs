using FluentValidation;
using Spotless.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Spotless.API.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                StatusCode = GetStatusCode(exception),
                Message = GetMessage(exception),
                Errors = GetErrors(exception)
            };

            response.StatusCode = (int)errorResponse.StatusCode;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(errorResponse, jsonOptions);
            return response.WriteAsync(json);
        }

        private static HttpStatusCode GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ValidationException => HttpStatusCode.BadRequest,
                UnauthorizedException => HttpStatusCode.Unauthorized,
                ForbiddenException => HttpStatusCode.Forbidden,
                NotFoundException => HttpStatusCode.NotFound,
                ConflictException => HttpStatusCode.Conflict,
                PaymentFailedException => HttpStatusCode.PaymentRequired,
                InsufficientWalletBalanceException => HttpStatusCode.BadRequest,
                DomainException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };
        }

        private static string GetMessage(Exception exception)
        {
            return exception switch
            {
                ValidationException => "Validation failed. Please check the errors.",
                UnauthorizedException => exception.Message,
                ForbiddenException => exception.Message,
                NotFoundException => exception.Message,
                ConflictException => exception.Message,
                PaymentFailedException => exception.Message,
                InsufficientWalletBalanceException => exception.Message,
                DomainException => exception.Message,
                UnauthorizedAccessException => exception.Message,
                KeyNotFoundException => exception.Message,
                ArgumentException => exception.Message,
                InvalidOperationException => exception.Message,
                _ => "An error occurred while processing your request."
            };
        }

        private static Dictionary<string, string[]>? GetErrors(Exception exception)
        {
            if (exception is ValidationException validationException)
            {
                return validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
            }

            return null;
        }

        private class ErrorResponse
        {
            public HttpStatusCode StatusCode { get; set; }
            public string Message { get; set; } = string.Empty;
            public Dictionary<string, string[]>? Errors { get; set; }
        }
    }
}


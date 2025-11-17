using Microsoft.Extensions.Logging;
using Spotless.Application.Interfaces;

namespace Spotless.Infrastructure.Services
{

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            // --- This is a MOCK implementation. ---
            _logger.LogInformation($"--- EMAIL SENT MOCK ---");
            _logger.LogInformation($"To: {toEmail}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Body Snippet: {body.Substring(0, Math.Min(200, body.Length))}...");
            _logger.LogInformation($"-------------------------");

            // In a real application, you would integrate a third-party email provider here.
            return Task.CompletedTask;
        }
    }
}
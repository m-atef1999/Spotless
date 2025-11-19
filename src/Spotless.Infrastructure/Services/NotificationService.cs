using Microsoft.Extensions.Logging;
using Spotless.Application.Interfaces;

namespace Spotless.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IEmailService emailService, ISmsService smsService, ILogger<NotificationService> logger)
        {
            _emailService = emailService;
            _smsService = smsService;
            _logger = logger;
        }

        public async Task SendEmailNotificationAsync(string email, string subject, string message)
        {
            await _emailService.SendEmailAsync(email, subject, message);
            _logger.LogInformation("Email notification sent to {Email}", email);
        }

        public async Task SendSmsNotificationAsync(string phoneNumber, string message)
        {
            _logger.LogInformation("SMS notification sent to {PhoneNumber}: {Message}", phoneNumber, message);
            await Task.CompletedTask;
        }

        public async Task SendPushNotificationAsync(string userId, string title, string message)
        {
            _logger.LogInformation("Push notification sent to {UserId}: {Title} - {Message}", userId, title, message);
            await Task.CompletedTask;
        }
    }
}
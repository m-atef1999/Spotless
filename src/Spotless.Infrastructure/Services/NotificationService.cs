using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;

namespace Spotless.Infrastructure.Services
{
    public class NotificationService(
        IEmailService emailService,
        ISmsService smsService,
        IMessageSender messageSender,
        IPushNotificationSender pushSender,
        IOptions<NotificationSettings> settings,
        ILogger<NotificationService> logger) : INotificationService
    {
        private readonly IEmailService _emailService = emailService;
        private readonly ISmsService _smsService = smsService;
        private readonly IMessageSender _messageSender = messageSender;
        private readonly IPushNotificationSender _pushSender = pushSender;
        private readonly NotificationSettings _settings = settings.Value;
        private readonly ILogger<NotificationService> _logger = logger;

        public async Task SendEmailNotificationAsync(string to, string subject, string body)
        {
            if (!_settings.EnableEmailNotifications) return;
            try
            {
                await _emailService.SendEmailAsync(to, subject, body);
                _logger.LogInformation("Email notification sent to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
            }
        }

        public async Task SendSmsNotificationAsync(string phoneNumber, string message)
        {
            if (!_settings.EnableSmsNotifications) return;
            try
            {
                await _messageSender.SendSmsAsync(phoneNumber, message);
                _logger.LogInformation("SMS notification sent to {PhoneNumber}", phoneNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS to {PhoneNumber}", phoneNumber);
            }
        }

        public async Task SendPushNotificationAsync(string userId, string title, string message)
        {
            try
            {
                await _pushSender.SendNotificationAsync(userId, title, message);
                _logger.LogInformation("Push notification sent to User {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send push notification to User {UserId}", userId);
            }
        }

        public async Task SendWhatsAppNotificationAsync(string phoneNumber, string message)
        {
            try
            {
                await _messageSender.SendWhatsAppAsync(phoneNumber, message);
                _logger.LogInformation("WhatsApp notification sent to {PhoneNumber}", phoneNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send WhatsApp to {PhoneNumber}", phoneNumber);
            }
        }
    }
}

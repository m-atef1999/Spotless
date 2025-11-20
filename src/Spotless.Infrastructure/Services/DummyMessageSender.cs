using Microsoft.Extensions.Logging;
using Spotless.Application.Interfaces;

namespace Spotless.Infrastructure.Services
{
    /// <summary>
    /// Fallback Dummy implementation when Twilio is not configured or for development/testing.
    /// </summary>
    public class DummyMessageSender(ILogger<DummyMessageSender> logger) : IMessageSender
    {
        private readonly ILogger<DummyMessageSender> _logger = logger;

        public Task SendSmsAsync(string phoneNumber, string message)
        {
            _logger.LogInformation("[MOCK SMS] To: {Phone} Message: {Message}", phoneNumber, message);
            return Task.CompletedTask;
        }

        public Task SendWhatsAppAsync(string phoneNumber, string message)
        {
            _logger.LogInformation("[MOCK WhatsApp] To: {Phone} Message: {Message}", phoneNumber, message);
            return Task.CompletedTask;
        }
    }
}

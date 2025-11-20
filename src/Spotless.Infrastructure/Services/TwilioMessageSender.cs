using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Spotless.Infrastructure.Services
{
    /// <summary>
    /// Real Twilio SDK integration for SMS and WhatsApp sending.
    /// </summary>
    public class TwilioMessageSender : IMessageSender
    {
        private readonly ILogger<TwilioMessageSender> _logger;
        private readonly SmsSettings? _smsSettings;
        private readonly WhatsAppSettings? _whatsAppSettings;
        private readonly bool _isConfigured;

        public TwilioMessageSender(ILogger<TwilioMessageSender> logger, IOptions<SmsSettings> smsOptions, IOptions<WhatsAppSettings> waOptions)
        {
            _logger = logger;
            _smsSettings = smsOptions?.Value;
            _whatsAppSettings = waOptions?.Value;

            _isConfigured = !string.IsNullOrEmpty(_smsSettings?.TwilioAccountSid) && 
                           !string.IsNullOrEmpty(_smsSettings?.TwilioAuthToken);

            if (_isConfigured && _smsSettings != null)
            {
                TwilioClient.Init(_smsSettings.TwilioAccountSid, _smsSettings.TwilioAuthToken);
                _logger.LogInformation("Twilio client initialized successfully");
            }
            else
            {
                _logger.LogWarning("Twilio is not configured (missing AccountSid/AuthToken)");
            }
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            if (!_isConfigured)
            {
                _logger.LogWarning("[SMS] Twilio not configured; SMS not sent to {Phone}", phoneNumber);
                return;
            }

            if (string.IsNullOrEmpty(_smsSettings?.FromNumber))
            {
                _logger.LogWarning("[SMS] Twilio FromNumber not configured; SMS not sent to {Phone}", phoneNumber);
                return;
            }

            try
            {
                var result = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(_smsSettings.FromNumber),
                    to: new PhoneNumber(phoneNumber)
                );
                _logger.LogInformation("SMS sent successfully to {Phone}, MessageSid: {Sid}", phoneNumber, result.Sid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS to {Phone}", phoneNumber);
            }
        }

        public async Task SendWhatsAppAsync(string phoneNumber, string message)
        {
            if (!_isConfigured)
            {
                _logger.LogWarning("[WhatsApp] Twilio not configured; WhatsApp not sent to {Phone}", phoneNumber);
                return;
            }

            if (string.IsNullOrEmpty(_whatsAppSettings?.FromNumber))
            {
                _logger.LogWarning("[WhatsApp] Twilio FromNumber not configured; WhatsApp not sent to {Phone}", phoneNumber);
                return;
            }

            try
            {
                // Twilio WhatsApp API requires "whatsapp:" prefix on both from and to
                var result = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber($"whatsapp:{_whatsAppSettings.FromNumber}"),
                    to: new PhoneNumber($"whatsapp:{phoneNumber}")
                );
                _logger.LogInformation("WhatsApp sent successfully to {Phone}, MessageSid: {Sid}", phoneNumber, result.Sid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send WhatsApp to {Phone}", phoneNumber);
            }
        }
    }
}

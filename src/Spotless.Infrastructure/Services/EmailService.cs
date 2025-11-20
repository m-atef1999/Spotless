using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Spotless.Infrastructure.Services
{

    public class EmailService(ILogger<EmailService> logger, IOptions<EmailSettings> options) : IEmailService
    {
        private readonly ILogger<EmailService> _logger = logger;
        private readonly EmailSettings _settings = options?.Value ?? new EmailSettings();

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                using var msg = new MailMessage();
                msg.From = new MailAddress(_settings.FromEmail, _settings.FromName);
                msg.To.Add(new MailAddress(toEmail));
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = isHtml;

                using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort);
                client.EnableSsl = _settings.EnableSsl;

                if (!string.IsNullOrEmpty(_settings.SmtpUsername))
                {
                    client.Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword);
                }

                await client.SendMailAsync(msg);

                _logger.LogInformation("Email sent to {Email} via SMTP server {SmtpServer}", toEmail, _settings.SmtpServer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }
    }
}

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Spotless.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Spotless.Infrastructure.Services
{
    /// <summary>
    /// Background service that subscribes to RabbitMQ queues and processes jobs.
    /// </summary>
    public class BackgroundJobProcessor(IMessageBroker messageBroker, IServiceScopeFactory scopeFactory, ILogger<BackgroundJobProcessor> logger) : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly IMessageBroker _messageBroker = messageBroker;
        private readonly ILogger<BackgroundJobProcessor> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Job Processor starting...");

            try
            {
                // Check if using NoOpMessageBroker (RabbitMQ disabled)
                if (_messageBroker is NoOpMessageBroker)
                {
                    _logger.LogInformation("RabbitMQ is not available - Background Job Processor will not process any jobs");
                    await Task.Delay(Timeout.Infinite, stoppingToken);
                    return;
                }

                // Subscribe to notification queue
                await _messageBroker.SubscribeAsync<NotificationJob>(
                    "spotless.notifications",
                    async job => await ProcessNotificationJobAsync(job),
                    stoppingToken
                );

                // Subscribe to telemetry queue
                await _messageBroker.SubscribeAsync<TelemetryJob>(
                    "spotless.telemetry",
                    async job => await ProcessTelemetryJobAsync(job),
                    stoppingToken
                );

                // Subscribe to analytics queue
                await _messageBroker.SubscribeAsync<AnalyticsJob>(
                    "spotless.analytics",
                    async job => await ProcessAnalyticsJobAsync(job),
                    stoppingToken
                );

                // Subscribe to billing queue
                await _messageBroker.SubscribeAsync<BillingJob>(
                    "spotless.billing",
                    async job => await ProcessBillingJobAsync(job),
                    stoppingToken
                );

                _logger.LogInformation("Background Job Processor subscribed to all queues");

                // Keep the service running
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Background Job Processor stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Background Job Processor");
                throw;
            }
        }

        private async Task ProcessNotificationJobAsync(NotificationJob job)
        {
            using var scope = _scopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            try
            {
                _logger.LogInformation("Processing notification job {CorrelationId}: Email={Email}, SMS={SMS}, WhatsApp={WA}",
                    job.CorrelationId, job.SendEmail, job.SendSms, job.SendWhatsApp);

                if (job.SendEmail && !string.IsNullOrEmpty(job.Email))
                {
                    await notificationService.SendEmailNotificationAsync(job.Email, job.EmailSubject, job.EmailBody);
                }

                if (job.SendSms && !string.IsNullOrEmpty(job.PhoneNumber))
                {
                    await notificationService.SendSmsNotificationAsync(job.PhoneNumber, job.SmsMessage);
                }

                if (job.SendWhatsApp && !string.IsNullOrEmpty(job.PhoneNumber))
                {
                    await notificationService.SendWhatsAppNotificationAsync(job.PhoneNumber, job.WhatsAppMessage);
                }

                _logger.LogInformation("Notification job {CorrelationId} completed", job.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notification job {CorrelationId}", job.CorrelationId);
                throw;
            }
        }

        private async Task ProcessTelemetryJobAsync(TelemetryJob job)
        {
            try
            {
                _logger.LogInformation("Processing telemetry job {CorrelationId}: Driver={DriverId}, Location=({Lat},{Lon})",
                    job.CorrelationId, job.DriverId, job.Latitude, job.Longitude);

                // TODO: Persist telemetry to time-series database or data warehouse
                // For now, just log it
                await Task.CompletedTask;

                _logger.LogInformation("Telemetry job {CorrelationId} completed", job.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing telemetry job {CorrelationId}", job.CorrelationId);
                throw;
            }
        }

        private async Task ProcessAnalyticsJobAsync(AnalyticsJob job)
        {
            try
            {
                _logger.LogInformation("Processing analytics job {CorrelationId}: Event={EventType}, Entity={EntityId}",
                    job.CorrelationId, job.EventType, job.EntityId);

                // TODO: Persist to analytics database/data lake for BI/ML pipelines
                // For now, just log it
                await Task.CompletedTask;

                _logger.LogInformation("Analytics job {CorrelationId} completed", job.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing analytics job {CorrelationId}", job.CorrelationId);
                throw;
            }
        }

        private async Task ProcessBillingJobAsync(BillingJob job)
        {
            try
            {
                _logger.LogInformation("Processing billing job {CorrelationId}: Order={OrderId}, Amount={Amount} {Currency}",
                    job.CorrelationId, job.OrderId, job.Amount, job.Currency);

                // TODO: Forward to billing system, update ledger, reconcile payments
                // For now, just log it
                await Task.CompletedTask;

                _logger.LogInformation("Billing job {CorrelationId} completed", job.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing billing job {CorrelationId}", job.CorrelationId);
                throw;
            }
        }
    }
}

using Microsoft.Extensions.Logging;
using Spotless.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Spotless.Infrastructure.Services
{
    /// <summary>
    /// No-op implementation of IMessageBroker used when RabbitMQ is disabled.
    /// Allows the application to run without RabbitMQ for development/testing.
    /// </summary>
    public class NoOpMessageBroker : IMessageBroker
    {
        private readonly ILogger<NoOpMessageBroker> _logger;

        public NoOpMessageBroker(ILogger<NoOpMessageBroker> logger)
        {
            _logger = logger;
            _logger.LogInformation("NoOpMessageBroker initialized - RabbitMQ functionality is disabled");
        }

        public Task PublishAsync<T>(string queueName, T message) where T : class
        {
            _logger.LogDebug("NoOp: Message of type {MessageType} would be published to queue {QueueName}", typeof(T).Name, queueName);
            return Task.CompletedTask;
        }

        public Task SubscribeAsync<T>(string queueName, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class
        {
            _logger.LogDebug("NoOp: Subscription to queue {QueueName} would be created", queueName);
            return Task.CompletedTask;
        }
    }
}

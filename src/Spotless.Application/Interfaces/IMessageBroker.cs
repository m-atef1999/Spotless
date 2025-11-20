using System;
using System.Threading;
using System.Threading.Tasks;

namespace Spotless.Application.Interfaces
{
    /// <summary>
    /// Abstraction for message broker (RabbitMQ, Service Bus, etc.) for decoupling async tasks.
    /// </summary>
    public interface IMessageBroker
    {
        /// <summary>
        /// Publishes a message to a queue for asynchronous processing.
        /// </summary>
        Task PublishAsync<T>(string queueName, T message) where T : class;

        /// <summary>
        /// Subscribes to a queue and processes messages with the provided handler.
        /// </summary>
        Task SubscribeAsync<T>(string queueName, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class;
    }

    /// <summary>
    /// Base interface for background job messages.
    /// </summary>
    public interface IBackgroundJob
    {
        Guid CorrelationId { get; }
        DateTime CreatedAt { get; }
    }
}

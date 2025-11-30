using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;

namespace Spotless.Infrastructure.Services
{
    /// <summary>
    /// RabbitMQ-based message broker for decoupled async processing.
    /// Implements lazy connection initialization to handle cases where RabbitMQ is unavailable.
    /// </summary>
    public class RabbitMqMessageBroker : IMessageBroker, IDisposable
    {
        private IConnection? _connection;
        private IModel? _channel;
        private readonly RabbitMqSettings _settings;
        private readonly ILogger<RabbitMqMessageBroker> _logger;
        private bool _connectionInitialized = false;
        private bool _connectionFailed = false;
        private readonly object _lockObject = new();

        public RabbitMqMessageBroker(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqMessageBroker> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Lazily initializes the RabbitMQ connection on first use.
        /// Returns false if connection fails, allowing graceful degradation.
        /// </summary>
        private bool EnsureConnected()
        {
            if (_connectionInitialized || _connectionFailed)
                return !_connectionFailed;

            lock (_lockObject)
            {
                if (_connectionInitialized || _connectionFailed)
                    return !_connectionFailed;

                try
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = _settings.HostName,
                        Port = _settings.Port,
                        UserName = _settings.UserName,
                        Password = _settings.Password,
                        VirtualHost = _settings.VirtualHost,
                        AutomaticRecoveryEnabled = true,
                        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                        RequestedConnectionTimeout = TimeSpan.FromSeconds(5),
                        ContinuationTimeout = TimeSpan.FromSeconds(5)
                    };

                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();

                    // Set QoS: prefetch only 1 message at a time to ensure fair distribution
                    _channel.BasicQos(0, 1, false);

                    _connectionInitialized = true;
                    _logger.LogInformation("RabbitMQ connection established at {HostName}:{Port}", _settings.HostName, _settings.Port);
                    return true;
                }
                catch (Exception ex)
                {
                    _connectionFailed = true;
                    _logger.LogError(ex, "Failed to initialize RabbitMQ connection to {HostName}:{Port}. Messages will not be persisted until RabbitMQ is available.", _settings.HostName, _settings.Port);
                    return false;
                }
            }
        }

        public async Task PublishAsync<T>(string queueName, T message) where T : class
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name must be provided", nameof(queueName));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            try
            {
                if (!EnsureConnected())
                {
                    _logger.LogWarning("RabbitMQ is not available. Message of type {MessageType} to queue {QueueName} will not be persisted.", typeof(T).Name, queueName);
                    await Task.CompletedTask;
                    return;
                }

                // Declare the queue (idempotent operation)
                _channel!.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.ContentType = "application/json";
                properties.Headers = new Dictionary<string, object>
                {
                    { "x-timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }
                };

                _channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: queueName,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation("Message published to queue {QueueName}, type: {MessageType}", queueName, typeof(T).Name);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to queue {QueueName}", queueName);
                throw;
            }
        }

        public async Task SubscribeAsync<T>(string queueName, Func<T, Task> handler, CancellationToken cancellationToken = default) where T : class
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name must be provided", nameof(queueName));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            try
            {
                if (!EnsureConnected())
                {
                    _logger.LogWarning("RabbitMQ is not available. Subscription to queue {QueueName} failed.", queueName);
                    await Task.CompletedTask;
                    return;
                }

                // Declare the queue
                _channel!.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);
                        var message = JsonSerializer.Deserialize<T>(json);

                        if (message != null)
                        {
                            await handler(message);
                            _channel.BasicAck(ea.DeliveryTag, false);
                            _logger.LogInformation("Message processed from queue {QueueName}, type: {MessageType}", queueName, typeof(T).Name);
                        }
                        else
                        {
                            _logger.LogWarning("Received null/invalid message from queue {QueueName}", queueName);
                            _channel.BasicNack(ea.DeliveryTag, false, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message from queue {QueueName}", queueName);
                        // Reject and requeue on error
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };

                _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                _logger.LogInformation("Subscribed to queue {QueueName}", queueName);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe to queue {QueueName}", queueName);
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                try
                {
                    _channel?.Close();
                }
                catch { }

                try
                {
                    _connection?.Close();
                }
                catch { }

                _channel?.Dispose();
                _connection?.Dispose();
                _logger.LogInformation("RabbitMQ connection closed");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error disposing RabbitMQ connection");
            }
        }
    }
}

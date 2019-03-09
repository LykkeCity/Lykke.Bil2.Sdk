using System;
using System.Collections.Generic;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Common;
using Lykke.Common.Log;
using RabbitMQ.Client;

namespace Lykke.Bil2.RabbitMq
{
    /// <inheritdoc />
    [PublicAPI]
    public class RabbitMqEndpoint : IRabbitMqEndpoint
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogFactory _logFactory;
        private readonly Uri _connectionString;
        private readonly ILog _log;
        private IConnection _connection;
        private IModel _publishingChannel;

        private readonly List<IDisposable> _subscribers;
        private readonly string _vhost;

        /// <summary>
        /// RabbitMq endpoint - represents RabbitMq connection and provides entry points
        /// to start listening for the messages and publish messages.
        /// </summary>
        public RabbitMqEndpoint(
            IServiceProvider serviceProvider,
            ILogFactory logFactory, 
            Uri connectionString, 
            string vhost = null)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logFactory = logFactory ?? throw new ArgumentNullException(nameof(logFactory));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _vhost = vhost;
            _log = logFactory.CreateLog(this);

            _subscribers = new List<IDisposable>();
        }

        /// <inheritdoc />
        public void Start()
        {
            if (_connection != null)
            {
                throw new InvalidOperationException("RabbitMqEndpoint already has been be started");
            }

            var factory = new ConnectionFactory
            {
                Uri = _connectionString,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                UseBackgroundThreadsForIO = true,
                VirtualHost = string.IsNullOrWhiteSpace(_vhost) ? "/" : _vhost
            };

            var connectionName = $"{AppEnvironment.Name} {AppEnvironment.Version}";

            _connection = factory.CreateConnection(connectionName);

            _publishingChannel = _connection.CreateModel();
        }

        /// <inheritdoc />
        public void DeclareExchange(string exchangeName)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))
            {
                throw new ArgumentException("Should be not empty string", nameof(exchangeName));
            }
            if (_connection == null)
            {
                throw new InvalidOperationException("RabbitMqEndpoint should be started first");
            }

            _log.Info($"Declaring exchange {exchangeName}...");

            _publishingChannel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true);
        }

        /// <inheritdoc />
        public IMessagePublisher CreatePublisher(string exchangeName, string correlationId = null)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))
            {
                throw new ArgumentException("Should be not empty string", nameof(exchangeName));
            }
            if (_connection == null)
            {
                throw new InvalidOperationException("RabbitMqEndpoint should be started first");
            }

            return new MessagePublisher(_publishingChannel, exchangeName, correlationId);
        }

        /// <inheritdoc />
        public void StartListening(
            string listeningExchangeName,
            string listeningRoute,
            IMessageSubscriptionsRegistry subscriptionsRegistry,
            string replyExchangeName = null,
            int parallelism = 1)
        {
            if (string.IsNullOrWhiteSpace(listeningExchangeName))
            {
                throw new ArgumentException("Should be not empty string", nameof(listeningExchangeName));
            }
            if (string.IsNullOrWhiteSpace(listeningRoute))
            {
                throw new ArgumentException("Should be not empty string", nameof(listeningRoute));
            }
            if (subscriptionsRegistry == null)
            {
                throw new ArgumentNullException(nameof(subscriptionsRegistry));
            }
            if (_connection == null)
            {
                throw new InvalidOperationException("RabbitMqEndpoint should be started first");
            }

            var subscriber = new MessageSubscriber
            (
                _serviceProvider,
                _logFactory,
                _connection,
                listeningExchangeName,
                $"{listeningExchangeName}.{listeningRoute}",
                subscriptionsRegistry,
                parallelism
            );

            if (replyExchangeName != null)
            {
                subscriber.WithRepliesPublisher(correlationId => CreatePublisher(replyExchangeName, correlationId));
            }

            _subscribers.Add(subscriber);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Dispose();
            }

            _publishingChannel?.Close();
            _publishingChannel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();
        }
    }
}

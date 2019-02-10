using System;
using System.Collections.Generic;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common;
using Lykke.Common.Log;
using RabbitMQ.Client;

namespace Lykke.Blockchains.Integrations.RabbitMq
{
    /// <inheritdoc />
    [PublicAPI]
    public class RabbitMqEndpoint : IRabbitMqEndpoint
    {
        private readonly ILogFactory _logFactory;
        private readonly Uri _connectionString;
        private readonly ILog _log;
        private IConnection _connection;
        private IModel _publishingChannel;

        private readonly List<IDisposable> _subscribers;

        /// <summary>
        /// RabbitMq endpoint - represents RabbitMq connection and provides entrypoints
        /// to start listening for the messages and publish messages.
        /// </summary>
        public RabbitMqEndpoint(
            ILogFactory logFactory, 
            Uri connectionString)
        {
            _logFactory = logFactory;
            _connectionString = connectionString;
            _log = logFactory.CreateLog(this);

            _subscribers = new List<IDisposable>();
        }

        /// <inheritdoc />
        public void Start()
        {
            var factory = new ConnectionFactory
            {
                Uri = _connectionString,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                UseBackgroundThreadsForIO = true
            };

            var connectionName = $"{AppEnvironment.Name} {AppEnvironment.Version}";

            _connection = factory.CreateConnection(connectionName);

            _publishingChannel = _connection.CreateModel();
        }

        /// <inheritdoc />
        public void DeclareExchange(string exchangeName)
        {
            _log.Info($"Declaring exchange {exchangeName}...");

            _publishingChannel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, true);
        }

        /// <inheritdoc />
        public IMessagePublisher CreatePublisher(string exchangeName, string corellationId = null)
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("RabbitMqEndpoint should be started first");
            }

            return new MessagePublisher(_publishingChannel, exchangeName, corellationId);
        }

        /// <inheritdoc />
        public void StartListening(
            string listeningExchangeName,
            string listeningRoute,
            IMessageSubscriptionsRegistry subscriptionsRegistry,
            string replyExchangeName = null,
            int parallelism = 1)
        {
            if (_connection == null)
            {
                throw new InvalidOperationException("RabbitMqEndpoint should be started first");
            }

            var subscriber = new MessageSubscriber
            (
                _logFactory,
                _connection,
                listeningExchangeName,
                $"{listeningExchangeName}.{listeningRoute}",
                subscriptionsRegistry,
                parallelism
            );

            if (replyExchangeName != null)
            {
                subscriber.WithRepliesPublisher(corellationId => CreatePublisher(replyExchangeName, corellationId));
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

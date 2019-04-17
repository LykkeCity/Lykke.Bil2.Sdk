using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Bil2.RabbitMq.MessagePack;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Bil2.RabbitMq.Subscription.Core;
using Lykke.Common;
using Lykke.Common.Log;
using Lykke.Numerics.MessagePack;
using MessagePack;
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

        private readonly List<MessageSubscriber> _subscribers;
        private readonly string _vhost;

        private readonly ICompositeFormatterResolver _formatterResolver;

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
            _formatterResolver = new CompositeFormatterResolver();
            _subscribers = new List<MessageSubscriber>();
            
            _formatterResolver.RegisterResolvers(MoneyResolver.Instance);
        }

        /// <inheritdoc />
        public void Initialize()
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

            _publishingChannel.ExchangeDeclare(exchangeName, ExchangeType.Topic, true);
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

            return new MessagePublisher(_publishingChannel, exchangeName, correlationId, _formatterResolver);
        }

        /// <inheritdoc />
        public void RegisterMessagePackFormatterResolvers(params IFormatterResolver[] resolvers)
        {
            _formatterResolver.RegisterResolvers(resolvers);
        }

        /// <inheritdoc />
        public void Subscribe(
            string listeningExchangeName,
            string listeningRoute,
            IMessageSubscriptionsRegistry subscriptionsRegistry,
            TimeSpan? defaultRetryTimeout = null,
            int internalQueueMaxCapacity = 1000,
            TimeSpan? maxMessageAgeForRetry = null,
            int maxMessageRetryCount = 5,
            int messageConsumersCount = 1,
            int messageProcessorsCount = 1,
            string replyExchangeName = null)
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

            if (internalQueueMaxCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(internalQueueMaxCapacity), internalQueueMaxCapacity, "Should be a positive number.");
            }
            
            if (maxMessageRetryCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxMessageRetryCount), maxMessageRetryCount, "Should be a positive number.");
            }
            
            if (messageConsumersCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(messageConsumersCount), messageConsumersCount, "Should be a positive number.");
            }
            
            if (messageProcessorsCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(messageProcessorsCount), messageProcessorsCount, "Should be a positive number.");
            }
            
            
            if (_connection == null)
            {
                throw new InvalidOperationException("RabbitMqEndpoint should be started first");
            }

            var listeningQueueName = $"{listeningExchangeName}.{listeningRoute}";

            Func<string, IMessagePublisher> repliesPublisher = null;
            
            if (replyExchangeName != null)
            {
                repliesPublisher = correlationId => CreatePublisher(replyExchangeName, correlationId);
            }
            
            var subscriber = MessageSubscriber.Create
            (
                _connection,
                defaultRetryTimeout ?? TimeSpan.FromSeconds(30),
                listeningExchangeName,
                _formatterResolver,
                internalQueueMaxCapacity,
                _logFactory,
                maxMessageAgeForRetry ?? TimeSpan.FromMinutes(10),
                maxMessageRetryCount,
                messageConsumersCount,
                messageProcessorsCount,
                listeningQueueName,
                repliesPublisher,
                _serviceProvider,
                subscriptionsRegistry
            );
            
            _subscribers.Add(subscriber);
        }

        /// <inheritdoc />
        public void StartListening()
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.StartListening();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Parallel.ForEach(_subscribers, x => x.Dispose());
            
            _publishingChannel?.Close();
            _publishingChannel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();
        }
    }
}

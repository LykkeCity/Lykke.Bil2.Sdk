using System;
using System.Collections.Generic;
using Common.Log;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.RabbitMq;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Common.Log;

namespace Lykke.Bil2.Client.BlocksReader.Services
{
    internal class BlocksReaderClient : IBlocksReaderClient
    {
        private ILog _log;
        private readonly IRabbitMqEndpoint _endpoint;
        private readonly IServiceProvider _serviceProvider;
        private readonly IReadOnlyCollection<string> _integrationNames;
        private readonly string _clientName;
        private readonly TimeSpan? _defaultFirstLevelRetryTimeout;
        private readonly TimeSpan? _maxFirstLevelRetryMessageAge;
        private readonly int _maxFirstLevelRetryCount;
        private readonly int _firstLevelRetryQueueCapacity;
        private readonly int _processingQueueCapacity;
        private readonly int _messageConsumersCount;
        private readonly int _messageProcessorsCount;
        private readonly IReadOnlyCollection<IMessageFilter> _messageFilters;

        public BlocksReaderClient(
            ILogFactory logFactory,
            IRabbitMqEndpoint endpoint,
            IServiceProvider serviceProvider,
            IReadOnlyCollection<string> integrationNames,
            string clientName,
            TimeSpan? defaultFirstLevelRetryTimeout,
            TimeSpan? maxFirstLevelRetryMessageAge,
            int maxFirstLevelRetryCount,
            int firstLevelRetryQueueCapacity,
            int processingQueueCapacity,
            int messageConsumersCount,
            int messageProcessorsCount,
            IReadOnlyCollection<IMessageFilter> messageFilters)
        {
            if (logFactory == null)
            {
                throw new ArgumentNullException(nameof(logFactory));
            }
            if (string.IsNullOrWhiteSpace(clientName))
            {
                throw new ArgumentException("Should be not empty string", nameof(clientName));
            }

            _log = logFactory.CreateLog(this);
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _integrationNames = integrationNames ?? throw new ArgumentNullException(nameof(integrationNames));
            _clientName = clientName;
            _defaultFirstLevelRetryTimeout = defaultFirstLevelRetryTimeout;
            _maxFirstLevelRetryMessageAge = maxFirstLevelRetryMessageAge;
            _maxFirstLevelRetryCount = maxFirstLevelRetryCount;
            _firstLevelRetryQueueCapacity = firstLevelRetryQueueCapacity;
            _processingQueueCapacity = processingQueueCapacity;
            _messageConsumersCount = messageConsumersCount;
            _messageProcessorsCount = messageProcessorsCount;
            _messageFilters = messageFilters ?? throw new ArgumentNullException(nameof(messageFilters));
        }

        public void Initialize()
        {
            _endpoint.Initialize();

            foreach (var integrationName in _integrationNames)
            {
                _log.Info($"Initializing RabbitMq endpoint for the blockchain integration {integrationName}...");

                var kebabIntegrationName = integrationName.CamelToKebab();
                var commandsExchangeName = RabbitMqExchangeNamesFactory.GetIntegrationCommandsExchangeName(kebabIntegrationName);
                var eventsExchangeName = RabbitMqExchangeNamesFactory.GetIntegrationEventsExchangeName(kebabIntegrationName);

                _endpoint.DeclareExchange(commandsExchangeName);
                _endpoint.DeclareExchange(eventsExchangeName);

                var subscriptions = new MessageSubscriptionsRegistry()
                    .Handle<BlockHeaderReadEvent, string>(o =>
                    {
                        o.WithHandler<IBlockEventsHandler>();
                        o.WithState(integrationName);
                    })
                    .Handle<BlockNotFoundEvent, string>(o =>
                    {
                        o.WithHandler<IBlockEventsHandler>();
                        o.WithState(integrationName);
                    })
                    .Handle<TransferAmountTransactionsBatchEvent, string>(o =>
                    {
                        o.WithHandler<IBlockEventsHandler>();
                        o.WithState(integrationName);
                    })
                    .Handle<TransferCoinsTransactionsBatchEvent, string>(o =>
                    {
                        o.WithHandler<IBlockEventsHandler>();
                        o.WithState(integrationName);
                    })
                    .Handle<LastIrreversibleBlockUpdatedEvent, string>(o =>
                    {
                        o.WithHandler<IBlockEventsHandler>();
                        o.WithState(integrationName);
                    });

                foreach (var filter in _messageFilters)
                {
                    subscriptions.AddFilter(filter);
                }

                _endpoint.Subscribe
                (
                    subscriptions,
                    eventsExchangeName,
                    $"bil-v2.{_clientName}",
                    _defaultFirstLevelRetryTimeout,
                    _maxFirstLevelRetryMessageAge,
                    _maxFirstLevelRetryCount,
                    _firstLevelRetryQueueCapacity,
                    _processingQueueCapacity,
                    _messageConsumersCount,
                    _messageProcessorsCount
                );
            }
        }

        public void StartListening()
        {
            _endpoint.StartListening();
        }

        public void Dispose()
        {
            _endpoint?.Dispose();
        }
    }
}

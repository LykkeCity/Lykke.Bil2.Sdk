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

        public BlocksReaderClient(
            ILogFactory logFactory,
            IRabbitMqEndpoint endpoint,
            IServiceProvider serviceProvider,
            IReadOnlyCollection<string> integrationNames,
            string clientName)
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
                    .Handle<TransferAmountTransactionExecutedEvent, string>(o =>
                    {
                        o.WithHandler<IBlockEventsHandler>();
                        o.WithState(integrationName);
                    })
                    .Handle<TransferCoinsTransactionExecutedEvent, string>(o =>
                    {
                        o.WithHandler<IBlockEventsHandler>();
                        o.WithState(integrationName);
                    })
                    .Handle<TransactionFailedEvent, string>(o =>
                    {
                        o.WithHandler<IBlockEventsHandler>();
                        o.WithState(integrationName);
                    })
                    .Handle<LastIrreversibleBlockUpdatedEvent, string>(o =>
                    {
                        o.WithHandler<IBlockEventsHandler>();
                        o.WithState(integrationName);
                    });

                _endpoint.Subscribe(
                    eventsExchangeName,
                    $"bil-v2.{_clientName}",
                    subscriptions);
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

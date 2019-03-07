using System;
using System.Collections.Generic;
using Common.Log;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.RabbitMq;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Common.Log;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Client.BlocksReader.Services
{
    internal class BlocksReaderClient : IBlocksReaderClient
    {
        private ILog _log;
        private readonly IRabbitMqEndpoint _endpoint;
        private readonly IServiceProvider _serviceProvider;
        private readonly IReadOnlyCollection<string> _integrationNames;
        private readonly string _clientName;
        private readonly int _listeningParallelism;
        
        public BlocksReaderClient(
            ILogFactory logFactory,
            IRabbitMqEndpoint endpoint,
            IServiceProvider serviceProvider,
            IReadOnlyCollection<string> integrationNames,
            string clientName,
            int listeningParallelism)
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

            if (listeningParallelism <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(listeningParallelism), listeningParallelism, "Should be positive number");
            }

            _listeningParallelism = listeningParallelism;
        }

        public void Start()
        {
            _endpoint.Start();

            foreach (var integrationName in _integrationNames)
            {
                _log.Info($"Registering blockchain integration {integrationName}");

                var kebabIntegrationName = integrationName.CamelToKebab();
                var commandsExchangeName = RabbitMqExchangeNamesFactory.GetIntegrationCommandsExchangeName(kebabIntegrationName);
                var eventsExchangeName = RabbitMqExchangeNamesFactory.GetIntegrationEventsExchangeName(kebabIntegrationName);

                _endpoint.DeclareExchange(commandsExchangeName);
                _endpoint.DeclareExchange(eventsExchangeName);

                var subscriptions = new MessageSubscriptionsRegistry()
                    .On<BlockHeaderReadEvent>((evt, publisher) => _serviceProvider.GetRequiredService<IBlockEventsHandler>().Handle(integrationName, evt))
                    .On<BlockNotFoundEvent>((evt, publisher) => _serviceProvider.GetRequiredService<IBlockEventsHandler>().Handle(integrationName, evt))
                    .On<TransferAmountTransactionExecutedEvent>((evt, publisher) => _serviceProvider.GetRequiredService<IBlockEventsHandler>().Handle(integrationName, evt))
                    .On<TransferCoinsTransactionExecutedEvent>((evt, publisher) => _serviceProvider.GetRequiredService<IBlockEventsHandler>().Handle(integrationName, evt))
                    .On<TransactionFailedEvent>((evt, publisher) => _serviceProvider.GetRequiredService<IBlockEventsHandler>().Handle(integrationName, evt))
                    .On<LastIrreversibleBlockUpdatedEvent>((evt, publisher) => _serviceProvider.GetRequiredService<IBlockEventsHandler>().Handle(integrationName, evt));

                _endpoint.StartListening(
                    eventsExchangeName,
                    $"bil-v2.{_clientName}",
                    subscriptions,
                    parallelism: _listeningParallelism);
            }
        }

        public void Dispose()
        {
            _endpoint?.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using Common.Log;
using Lykke.Blockchains.Integrations.Contract.BlocksReader.Events;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.RabbitMq;
using Lykke.Common.Log;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Blockchains.Integrations.Client.BlocksReader.Services
{
    internal class BlocksReaderClient : IBlocksReaderClient
    {
        private ILog _log;
        private readonly IRabbitMqEndpoint _endpoint;
        private readonly IServiceProvider _serviceProvider;
        private readonly IReadOnlyCollection<string> _integrationNames;
        private readonly int _listeningParallelism;
        
        public BlocksReaderClient(
            ILogFactory logFactory,
            IRabbitMqEndpoint endpoint,
            IServiceProvider serviceProvider,
            IReadOnlyCollection<string> integrationNames,
            int listeningParallelism)
        {
            if (logFactory == null)
            {
                throw new ArgumentNullException(nameof(logFactory));
            }

            _log = logFactory.CreateLog(this);
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _integrationNames = integrationNames ?? throw new ArgumentNullException(nameof(integrationNames));

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

                var kebabIntegrationName = IntegrationNameTools.ToKebab(integrationName);
                var commandsExchangeName = RabbitMqExchangeNamesFactory.GetIntegrationCommandsExchangeName(kebabIntegrationName);
                var eventsExchangeName = RabbitMqExchangeNamesFactory.GetIntegrationEventsExchangeName(kebabIntegrationName);

                _endpoint.DeclareExchange(commandsExchangeName);
                _endpoint.DeclareExchange(eventsExchangeName);

                var subscriptions = new MessageSubscriptionsRegistry()
                    .On<BlockHeaderReadEvent>((evt, publisher) => _serviceProvider.GetRequiredService<IBlockEventsHandler>().Handle(integrationName, evt))
                    .On<TransactionExecutedEvent>((evt, publisher) => _serviceProvider.GetRequiredService<IBlockEventsHandler>().Handle(integrationName, evt))
                    .On<TransactionFailedEvent>((evt, publisher) => _serviceProvider.GetRequiredService<IBlockEventsHandler>().Handle(integrationName, evt));

                _endpoint.StartListening(
                    eventsExchangeName,
                    "bil-v2.indexer",
                    subscriptions,
                    parallelism: _listeningParallelism);
            }
        }
    }
}

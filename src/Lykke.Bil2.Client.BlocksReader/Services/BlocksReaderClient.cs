using System;
using System.Collections.Generic;
using System.Linq;
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
        private class IntegrationExchanges
        {
            public string CommandsExchangeName { get; }
            public string EventsExchangeName { get; }

            public IntegrationExchanges(string commandsExchangeName, string eventsExchangeName)
            {
                CommandsExchangeName = commandsExchangeName;
                EventsExchangeName = eventsExchangeName;
            }
        }
        
        private ILog _log;
        private readonly IRabbitMqEndpoint _endpoint;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _clientName;
        private readonly int _listeningParallelism;
        private Dictionary<string, IntegrationExchanges> _integrationsExchangeNames;

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
            if (integrationNames == null)
            {
                throw new ArgumentNullException(nameof(integrationNames));
            }

            _log = logFactory.CreateLog(this);
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            
            _clientName = clientName;

            if (listeningParallelism <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(listeningParallelism), listeningParallelism, "Should be positive number");
            }

            _listeningParallelism = listeningParallelism;

            _integrationsExchangeNames = integrationNames
                .Select(name => new
                {
                    Name = name,
                    KebabName = name.CamelToKebab()
                })
                .ToDictionary(
                    x => x.Name,
                    x => new IntegrationExchanges(
                        RabbitMqExchangeNamesFactory.GetIntegrationCommandsExchangeName(x.KebabName),
                        RabbitMqExchangeNamesFactory.GetIntegrationEventsExchangeName(x.KebabName)));
        }

        public void StartSending()
        {
            _endpoint.Start();

            foreach (var (integrationName, exchangeNames) in _integrationsExchangeNames)
            {
                _log.Info($"Declaring commands exchange for the blockchain integration {integrationName}...");

                _endpoint.DeclareExchange(exchangeNames.CommandsExchangeName);
            }
        }

        public void StartListening()
        {
            foreach (var (integrationName, exchangeNames) in _integrationsExchangeNames)
            {
                _log.Info($"Starting events listening for the blockchain integration {integrationName}...");

                _endpoint.DeclareExchange(exchangeNames.EventsExchangeName);

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

                _endpoint.StartListening(
                    exchangeNames.EventsExchangeName,
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

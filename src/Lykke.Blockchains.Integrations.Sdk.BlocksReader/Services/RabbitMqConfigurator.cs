using System;
using Lykke.Blockchains.Integrations.Contract.BlocksReader.Commands;
using Lykke.Blockchains.Integrations.RabbitMq;
using Lykke.Blockchains.Integrations.RabbitMq.Subscription;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader.Services
{
    internal class RabbitMqConfigurator : IRabbitMqConfigurator
    {
        private readonly IRabbitMqEndpoint _rabbitMqEndpoint;
        private readonly IServiceProvider _serviceProvider;
        private readonly int _listeningParallelism;
        private readonly string _integrationName;

        public RabbitMqConfigurator(
            IRabbitMqEndpoint rabbitMqEndpoint,
            IServiceProvider serviceProvider,
            int listeningParallelism,
            string integrationName)
        {
            _rabbitMqEndpoint = rabbitMqEndpoint;
            _serviceProvider = serviceProvider;
            _listeningParallelism = listeningParallelism;
            _integrationName = integrationName;
        }

        public void Configure()
        {
            _rabbitMqEndpoint.Start();

            var commandsExchangeName = RabbitMqExchangeNamesFactory.GetIntegrationCommandsExchangeName(_integrationName);
            var eventsExchangeName = RabbitMqExchangeNamesFactory.GetIntegrationEventsExchangeName(_integrationName);

            _rabbitMqEndpoint.DeclareExchange(commandsExchangeName);
            _rabbitMqEndpoint.DeclareExchange(eventsExchangeName);

            var subscriptions = new MessageSubscriptionsRegistry()
                .On<ReadBlockCommand>((command, publisher) => _serviceProvider.GetRequiredService<ReadBlockCommandsHandler>().Handle(command, publisher));

            _rabbitMqEndpoint.StartListening(
                commandsExchangeName,
                $"bil-v2.bcn-{_integrationName}",
                subscriptions,
                eventsExchangeName,
                _listeningParallelism);
        }
    }
}

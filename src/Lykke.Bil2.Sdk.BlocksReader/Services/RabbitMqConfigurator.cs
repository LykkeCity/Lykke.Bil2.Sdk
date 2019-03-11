using Lykke.Bil2.Contract.BlocksReader.Commands;
using Lykke.Bil2.RabbitMq;
using Lykke.Bil2.RabbitMq.Subscription;

namespace Lykke.Bil2.Sdk.BlocksReader.Services
{
    internal class RabbitMqConfigurator : IRabbitMqConfigurator
    {
        private readonly IRabbitMqEndpoint _rabbitMqEndpoint;
        private readonly int _listeningParallelism;
        private readonly string _integrationName;

        public RabbitMqConfigurator(
            IRabbitMqEndpoint rabbitMqEndpoint,
            int listeningParallelism,
            string integrationName)
        {
            _rabbitMqEndpoint = rabbitMqEndpoint;
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
                .Handle<ReadBlockCommand>(o =>
                {
                    o.WithHandler<ReadBlockCommandsHandler>();
                });

            _rabbitMqEndpoint.StartListening(
                commandsExchangeName,
                $"bil-v2.bcn-{_integrationName}",
                subscriptions,
                eventsExchangeName,
                _listeningParallelism);
        }
    }
}

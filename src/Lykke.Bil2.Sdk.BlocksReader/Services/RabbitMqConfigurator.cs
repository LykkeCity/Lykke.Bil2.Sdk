using Lykke.Bil2.Contract.BlocksReader.Commands;
using Lykke.Bil2.RabbitMq;
using Lykke.Bil2.RabbitMq.Subscription;

namespace Lykke.Bil2.Sdk.BlocksReader.Services
{
    internal class RabbitMqConfigurator : IRabbitMqConfigurator
    {
        private readonly string _integrationName;
        private readonly int _listeningParallelism;
        private readonly int _processingParallelism;
        private readonly IRabbitMqEndpoint _rabbitMqEndpoint;

        public RabbitMqConfigurator(
            IRabbitMqEndpoint rabbitMqEndpoint,
            int listeningParallelism,
            int processingParallelism,
            string integrationName)
        {
            _integrationName = integrationName;
            _listeningParallelism = listeningParallelism;
            _processingParallelism = processingParallelism;
            _rabbitMqEndpoint = rabbitMqEndpoint;
        }

        public void Configure()
        {
            _rabbitMqEndpoint.Initialize();

            var commandsExchangeName = RabbitMqExchangeNamesFactory.GetIntegrationCommandsExchangeName(_integrationName);
            var eventsExchangeName = RabbitMqExchangeNamesFactory.GetIntegrationEventsExchangeName(_integrationName);

            _rabbitMqEndpoint.DeclareExchange(commandsExchangeName);
            _rabbitMqEndpoint.DeclareExchange(eventsExchangeName);

            var subscriptions = new MessageSubscriptionsRegistry()
                .Handle<ReadBlockCommand>(o =>
                {
                    o.WithHandler<ReadBlockCommandsHandler>();
                });

            _rabbitMqEndpoint.Subscribe(
                commandsExchangeName,
                $"bil-v2.bcn-{_integrationName}",
                subscriptions,
                messageConsumersCount: _listeningParallelism,
                messageProcessorsCount: _processingParallelism,
                replyExchangeName: eventsExchangeName);

            _rabbitMqEndpoint.StartListening();
        }
    }
}

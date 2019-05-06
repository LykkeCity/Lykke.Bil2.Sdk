using Lykke.Bil2.Contract.BlocksReader.Commands;
using Lykke.Bil2.RabbitMq;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Bil2.RabbitMq.Subscription.MessageFilters;
using Lykke.Bil2.Sdk.BlocksReader.Settings;

namespace Lykke.Bil2.Sdk.BlocksReader.Services
{
    internal class RabbitMqConfigurator : IRabbitMqConfigurator
    {
        private readonly string _integrationName;
        private readonly IRabbitMqEndpoint _rabbitMqEndpoint;
        private readonly BaseBlocksReaderRabbitMqSettings _rabbitMqSettings;

        public RabbitMqConfigurator(
            IRabbitMqEndpoint rabbitMqEndpoint,
            BaseBlocksReaderRabbitMqSettings rabbitMqSettings,
            string integrationName)
        {
            _integrationName = integrationName;
            _rabbitMqEndpoint = rabbitMqEndpoint;
            _rabbitMqSettings = rabbitMqSettings;
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
                })
                .AddFilter(new AppInsightTelemetryMessageFilter());

            _rabbitMqEndpoint.Subscribe(
                subscriptions,
                commandsExchangeName,
                $"bil-v2.bcn-{_integrationName}",
                messageConsumersCount: _rabbitMqSettings.MessageConsumersCount,
                messageProcessorsCount: _rabbitMqSettings.MessageProcessorsCount,
                defaultFirstLevelRetryTimeout: _rabbitMqSettings.DefaultFirstLevelRetryTimeout,
                maxFirstLevelRetryMessageAge: _rabbitMqSettings.MaxFirstLevelRetryMessageAge,
                maxFirstLevelRetryCount: _rabbitMqSettings.MaxFirstLevelRetryCount,
                firstLevelRetryQueueCapacity: _rabbitMqSettings.FirstLevelRetryQueueCapacity,
                processingQueueCapacity: _rabbitMqSettings.ProcessingQueueCapacity,
                replyExchangeName: eventsExchangeName);

            _rabbitMqEndpoint.StartListening();
        }
    }
}

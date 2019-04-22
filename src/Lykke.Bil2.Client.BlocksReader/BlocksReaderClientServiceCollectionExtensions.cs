using System;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Bil2.Client.BlocksReader.Options;
using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.RabbitMq;
using Lykke.Bil2.WebClient;
using Lykke.Common;
using Lykke.Common.Log;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Client.BlocksReader
{
    [PublicAPI]
    public static class BlocksReaderClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the messaging client of the blockchain integration blocks reader to the app services as <see cref="IBlocksReaderClient"/>
        /// </summary>
        /// <remarks>
        /// In order to use <see cref="IBlocksReaderClient"/>,
        /// call <see cref="IBlocksReaderClient.Initialize"/> and <see cref="IBlocksReaderClient.StartListening"/> first.
        /// </remarks>
        public static void AddBlocksReaderClient(
            this IServiceCollection services,
            Action<BlocksReaderClientOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var options = new BlocksReaderClientOptions();

            configureOptions.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.RabbitMqConnString))
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.RabbitMqConnString)} is required.");
            }
            if (!options.IntegrationNames.Any())
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.IntegrationNames)} at least one integration should be registered.");
            }
            if (options.BlockEventsHandlerFactory == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.BlockEventsHandlerFactory)} is required.");
            }

            services.AddSingleton<IRabbitMqEndpoint>(s => new RabbitMqEndpoint
            (
                s,
                s.GetRequiredService<ILogFactory>(),
                new Uri(options.RabbitMqConnString),
                options.RabbitVhost
            ));

            var clientName = string
                .Concat(AppEnvironment.Name.Split(".").Where(x => x != "Lykke" && x != "Service" && x != "Job"))
                .CamelToKebab();

            services.AddSingleton<IBlocksReaderClient>(s => new BlocksReaderClient
            (
                s.GetRequiredService<ILogFactory>(),
                s.GetRequiredService<IRabbitMqEndpoint>(),
                s,
                options.IntegrationNames,
                clientName,
                options.DefaultFirstLevelRetryTimeout,
                options.MaxFirstLevelRetryMessageAge,
                options.MaxFirstLevelRetryCount,
                options.FirstLevelRetryQueueCapacity,
                options.ProcessingQueueCapacity,
                options.MessageConsumersCount,
                options.MessageProcessorsCount,
                options.MessageFilters
            ));

            services.AddTransient<IBlocksReaderApiFactory, BlocksReaderApiFactory>();
            services.AddTransient(options.BlockEventsHandlerFactory);
        }

        /// <summary>
        /// Adds the HTTP client of the blockchain integration blocks reader to the app services as <see cref="IBlocksReaderWebApi"/>
        /// </summary>
        public static IServiceCollection AddBlocksReaderWebClient(
            this IServiceCollection services,
            Action<BlocksReaderWebClientOptions> configureClient)
        {
            return services.AddBlockchainIntegrationWebClient
                <
                    BlocksReaderWebClientOptions,
                    BlocksReaderWebClientIntegrationOptions,
                    IBlocksReaderWebApi,
                    IBlocksReaderWebApiProvider
                >
                (configureClient, apis => new BlocksReaderWebApiProvider(apis));
        }
    }
}

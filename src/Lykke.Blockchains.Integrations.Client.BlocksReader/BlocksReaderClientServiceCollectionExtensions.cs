using System;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Client.BlocksReader.Services;
using Lykke.Blockchains.Integrations.RabbitMq;
using Lykke.Common.Log;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Blockchains.Integrations.Client.BlocksReader
{
    [PublicAPI]
    public static class BlocksReaderClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds blockchain integration blocks reader client services.
        /// </summary>
        /// <remarks>
        /// In order to use <see cref="IBlocksReaderClient"/>, call <see cref="IBlocksReaderClient.Start"/> first
        /// </remarks>
        public static void AddBlocksReaderClient(
            this ServiceCollection services,
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
            if (options.MessageListeningParallelism <= 0)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.MessageListeningParallelism)} should be positive number. Actual: {options.MessageListeningParallelism}");
            }
            if (!options.IntegrationNames.Any())
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.IntegrationNames)} at least one integration should be registered.");
            }

            services.AddSingleton<IRabbitMqEndpoint>(s => new RabbitMqEndpoint
            (
                s.GetRequiredService<ILogFactory>(),
                new Uri(options.RabbitMqConnString)
            ));

            services.AddSingleton<IBlocksReaderClient>(s => new BlocksReaderClient
            (
                s.GetRequiredService<ILogFactory>(),
                s.GetRequiredService<IRabbitMqEndpoint>(),
                s,
                options.IntegrationNames,
                options.MessageListeningParallelism
            ));

            services.AddTransient<IBlocksReaderApiFactory, BlocksReaderApiFactory>();
        }
    }
}

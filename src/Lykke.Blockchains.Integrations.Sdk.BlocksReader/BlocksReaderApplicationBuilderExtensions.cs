using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Microsoft.AspNetCore.Builder;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader
{
    [PublicAPI]
    public static class BlocksReaderApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures a blockchain integration blocks reader application.
        /// </summary>
        public static IApplicationBuilder UseBlockchainBlocksReader(
            this IApplicationBuilder app,
            Action<BlocksReaderApplicationOptions> configureOptions)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var options = new BlocksReaderApplicationOptions();

            configureOptions.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.IntegrationName))
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.IntegrationName)} is required.");
            }

            app.UseBlockchainIntegrationConfiguration(integrationOptions =>
            {
                integrationOptions.ServiceName = $"{IntegrationNameTools.ToCamelCase(options.IntegrationName)} Blocks reader";
            });

            return app;
        }
    }
}

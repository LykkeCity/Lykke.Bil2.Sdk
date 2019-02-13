using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Microsoft.AspNetCore.Builder;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor
{
    [PublicAPI]
    public static class TransactionsExecutorApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures a blockchain integration transactions executor application.
        /// </summary>
        public static IApplicationBuilder UseBlockchainTransactionsExecutor(
            this IApplicationBuilder app,
            Action<TransactionsExecutorApplicationOptions> configureOptions)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var options = new TransactionsExecutorApplicationOptions();

            configureOptions.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.IntegrationName))
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.IntegrationName)} is required.");
            }

            app.UseBlockchainIntegrationConfiguration(integrationOptions =>
            {
                integrationOptions.ServiceName = $"{options.IntegrationName} Transactions executor";
            });

            return app;
        }
    }
}

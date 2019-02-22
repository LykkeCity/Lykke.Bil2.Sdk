using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;

namespace Lykke.Blockchains.Integrations.Sdk.SignService
{
    [PublicAPI]
    public static class SignServiceApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures a blockchain integration sign service application.
        /// </summary>
        public static IApplicationBuilder UseBlockchainSignService(
            this IApplicationBuilder app,
            Action<SignServiceApplicationOptions> configureOptions)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var options = new SignServiceApplicationOptions();

            configureOptions.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.IntegrationName))
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.IntegrationName)} is required.");
            }

            app.UseBlockchainIntegrationConfiguration(integrationOptions =>
            {
                integrationOptions.ServiceName = $"{options.IntegrationName} Sign service";
            });

            return app;
        }
    }
}

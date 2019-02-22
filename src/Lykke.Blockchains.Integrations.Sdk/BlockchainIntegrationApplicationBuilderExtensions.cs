using System;
using System.Net;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.Contract.Common.Responses;
using Lykke.Blockchains.Integrations.Sdk.Exceptions;
using Lykke.Sdk;
using Microsoft.AspNetCore.Builder;

namespace Lykke.Blockchains.Integrations.Sdk
{
    [PublicAPI]
    public static class BlockchainIntegrationApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures a blockchain integration service application.
        /// </summary>
        public static IApplicationBuilder UseBlockchainIntegrationConfiguration(
            this IApplicationBuilder app,
            Action<BlockchainIntegrationApplicationOptions> configureOptions)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }
            
            var options = new BlockchainIntegrationApplicationOptions();

            configureOptions.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.ServiceName))
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.ServiceName)} is required.");
            }

            app.UseLykkeConfiguration(lykkeSdkOptions =>
            {
                lykkeSdkOptions.DisableValidationExceptionMiddleware();
                lykkeSdkOptions.DefaultErrorHandler = ex =>
                {
                    switch (ex)
                    {
                        case OperationNotSupportedException _:
                            return BlockchainErrorResponse.CreateBrief(ex);
                        case RequestValidationException _:
                            return BlockchainErrorResponse.CreateBrief(ex);
                        default:
                            return BlockchainErrorResponse.Create(ex);
                    }
                };
                lykkeSdkOptions.UnhandledExceptionHttpStatusCodeResolver = ex =>
                {
                    switch (ex)
                    {
                        case OperationNotSupportedException _:
                            return HttpStatusCode.NotImplemented;
                        case RequestValidationException _:
                            return HttpStatusCode.BadRequest;
                        default:
                            return HttpStatusCode.InternalServerError;
                    }
                };

                lykkeSdkOptions.SwaggerOptions = new LykkeSwaggerOptions
                {
                    ApiTitle = $"{options.ServiceName} API",
                    ApiVersion = "v1"
                };
            });

            return app;
        }
    }
}

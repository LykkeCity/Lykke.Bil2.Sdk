using System;
using JetBrains.Annotations;
using Lykke.Bil2.Client.SignService.Options;
using Lykke.Bil2.Client.SignService.Services;
using Lykke.Bil2.WebClient;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Client.SignService
{
    [PublicAPI]
    public static class SignServiceClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the client of the blockchain integration sign service to the app services.
        /// Use <see cref="ISignServiceApiProvider"/> to access the API.
        /// </summary>
        public static IServiceCollection AddSignServiceClient(this IServiceCollection services, Action<SignServiceClientOptions> configureClient)
        {
            return services.AddBlockchainIntegrationWebClient
                <
                    SignServiceClientOptions,
                    SignServiceClientIntegrationOptions,
                    ISignServiceApi,
                    ISignServiceApiProvider
                >
                (configureClient, apis => new SignServiceApiProvider(apis));
        }
    }
}

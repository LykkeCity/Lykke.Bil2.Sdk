using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Bil2.WebClient.ClientOptions;
using Lykke.Bil2.WebClient.Services;
using Lykke.Common.Log;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.WebClient
{
    [PublicAPI]
    public static class BlockchainIntegrationWebClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the web client of the blockchain integration to the app services.
        /// Use <see cref="IBaseWebClientApiProvider{TApi}"/> to access the API of the client.
        /// </summary>
        public static IServiceCollection AddBlockchainIntegrationWebClient
            <
                TClientOptions, 
                TIntegrationOptions, 
                TApi,
                TApiProvider
            >
            (this IServiceCollection services, 
            Action<TClientOptions> configureClient,
            Func<IReadOnlyDictionary<string, TApi>, TApiProvider> apiProviderFactory)

            where TClientOptions : BaseWebClientOptions<TIntegrationOptions>, new()
            where TIntegrationOptions : BaseWebClientIntegrationOptions, new()
            where TApiProvider : class, IBaseWebClientApiProvider<TApi>
        {
            if (configureClient == null)
            {
                throw new ArgumentNullException(nameof(configureClient));
            }

            if (apiProviderFactory == null)
            {
                throw new ArgumentNullException(nameof(apiProviderFactory));
            }

            var clientOptions = new TClientOptions();

            configureClient.Invoke(clientOptions);

            if (!clientOptions.IntegrationOptions.Any())
            {
                throw new InvalidOperationException($"At least one integration should be added. Use {nameof(BaseWebClientOptions<TIntegrationOptions>.AddIntegration)} to add an integration");
            }

            services.AddSingleton(s =>
            {
                var apis = clientOptions.IntegrationOptions.ToDictionary(
                    x => x.Key,
                    x =>
                    {
                        var generator = HttpClientGeneratorFactory.Create(options =>
                        {
                            options.Url = x.Value.Url;
                            options.LogFactory = s.GetRequiredService<ILogFactory>();
                            options.Handlers = clientOptions.DelegatingHandlers.ToArray();
                            options.Timeout = clientOptions.Timeout;
                        });

                        return generator.Generate<TApi>();
                    });
                
                return apiProviderFactory.Invoke(apis);
            });

            return services;
        }
    }
}

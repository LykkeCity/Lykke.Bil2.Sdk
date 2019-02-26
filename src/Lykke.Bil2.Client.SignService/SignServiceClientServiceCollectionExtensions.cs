using System;
using System.Linq;
using System.Net.Http;
using JetBrains.Annotations;
using Lykke.Bil2.WebClient;
using Lykke.Common.Log;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Client.SignService
{
    [PublicAPI]
    public static class SignServiceClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the client of the blockchain integration sign service to the app services as <see cref="ISignServiceApi"/>
        /// </summary>
        public static IServiceCollection AddSignServiceClient(this IServiceCollection services, string url, params DelegatingHandler[] handlers)
        {
            if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _))
            {
                throw new ArgumentException($"Should be a valid URL. Actual value: {url}", nameof(url));
            }

            if (services.All(x => x.ServiceType != typeof(ILogFactory)))
            {
                throw new InvalidOperationException($"{typeof(ILogFactory)} service should be registered");
            }

            services.AddSingleton(s =>
            {
                var generator = HttpClientGeneratorFactory.Create(options =>
                {
                    options.Url = url;
                    options.LogFactory = s.GetRequiredService<ILogFactory>();
                    options.Handlers = handlers;
                });

                return generator.Generate<ISignServiceApi>();
            });

            return services;
        }
    }
}

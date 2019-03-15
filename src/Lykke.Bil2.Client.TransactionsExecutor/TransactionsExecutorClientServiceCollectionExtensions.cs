using System;
using System.Linq;
using System.Net.Http;
using JetBrains.Annotations;
using Lykke.Bil2.WebClient;
using Lykke.Common.Log;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Client.TransactionsExecutor
{
    [PublicAPI]
    public static class TransactionsExecutorClientServiceCollectionExtensions
    {

        /// <summary>
        /// Adds the client of the blockchain integration transactions executor to the app services as <see cref="ITransactionsExecutorApi"/>
        /// </summary>
        public static IServiceCollection AddTransactionsExecutorClient(this IServiceCollection services, string url
            , params DelegatingHandler[] handlers)
        {
            return AddTransactionsExecutorClient(services, url, null, handlers);
        }

        /// <summary>
            /// Adds the client of the blockchain integration transactions executor to the app services as <see cref="ITransactionsExecutorApi"/>
            /// </summary>
            public static IServiceCollection AddTransactionsExecutorClient(this IServiceCollection services, string url, TimeSpan? timeout = null, params DelegatingHandler[] handlers)
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
                    options.Handlers = handlers;
                    options.LogFactory = s.GetRequiredService<ILogFactory>();
                    options.Timeout = timeout;
                });

                return generator.Generate<ITransactionsExecutorApi>();
            });

            return services;
        }
    }
}

using System;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.WebClient;
using Lykke.Common.Log;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Blockchains.Integrations.Client.TransactionsExecutor
{
    [PublicAPI]
    public static class TransactionsExecutorClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the client of the blockchain integration transactions executor to the app services as <see cref="ITransactionsExecutorApi"/>
        /// </summary>
        public static IServiceCollection AddTransactionsExecutorClient(this IServiceCollection services, string url)
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
                var generator = HttpClientGeneratorFactory.Create(url, s.GetRequiredService<ILogFactory>());

                return generator.Generate<ITransactionsExecutorApi>();
            });

            return services;
        }
    }
}

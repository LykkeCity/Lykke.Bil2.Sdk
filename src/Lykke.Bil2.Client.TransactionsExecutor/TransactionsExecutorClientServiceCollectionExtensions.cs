using System;
using JetBrains.Annotations;
using Lykke.Bil2.Client.TransactionsExecutor.Options;
using Lykke.Bil2.Client.TransactionsExecutor.Services;
using Lykke.Bil2.WebClient;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Client.TransactionsExecutor
{
    [PublicAPI]
    public static class TransactionsExecutorClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the client of the blockchain integration transactions executor to the app services.
        /// Use <see cref="ITransactionsExecutorApiProvider"/> to access the API.
        /// </summary>
        public static IServiceCollection AddTransactionsExecutorClient(
            this IServiceCollection services,
            Action<TransactionsExecutorClientOptions> configureClient)
        {
            return services.AddBlockchainIntegrationWebClient
                <
                    TransactionsExecutorClientOptions,
                    TransactionsExecutorClientIntegrationOptions,
                    ITransactionsExecutorApi,
                    ITransactionsExecutorApiProvider
                >
                (configureClient, apis => new TransactionsExecutorApiProvider(apis));
        }
    }
}

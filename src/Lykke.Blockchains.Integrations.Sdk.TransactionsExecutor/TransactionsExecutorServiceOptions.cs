using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor
{
    /// <summary>
    /// Service provider options for a blockchain integration transactions executor.
    /// </summary>
    [PublicAPI]
    public class TransactionsExecutorServiceOptions
    {
        /// <summary>
        /// Required.
        /// Name of the integration.
        /// </summary>
        public string IntegrationName { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="IAddressValidator"/> implementation factory. 
        /// </summary>
        public Func<IServiceProvider, IAddressValidator> AddressValidatorFactory { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="IHealthProvider"/> implementation factory.
        /// </summary>
        public Func<IServiceProvider, IHealthProvider> HealthProviderFactory { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="IIntegrationInfoService"/> implementation factory.
        /// </summary>
        public Func<IServiceProvider, IIntegrationInfoService> IntegrationInfoServiceFactory { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="ITransactionEstimator"/> implementation factory.
        /// </summary>
        public Func<IServiceProvider, ITransactionEstimator> TransactionEstimatorFactory { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="ITransactionExecutor"/> implementation factory.
        /// </summary>
        public Func<IServiceProvider, ITransactionExecutor> TransactionExecutorFactory { get; set; }
    }
}

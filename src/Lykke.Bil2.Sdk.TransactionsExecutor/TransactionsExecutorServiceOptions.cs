using System;
using JetBrains.Annotations;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.Sdk.TransactionsExecutor.Settings;
using Lykke.SettingsReader;

namespace Lykke.Bil2.Sdk.TransactionsExecutor
{
    /// <summary>
    /// Service provider options for a blockchain integration transactions executor.
    /// </summary>
    [PublicAPI]
    public class TransactionsExecutorServiceOptions<TAppSettings>

        where TAppSettings : class, ITransactionsExecutorSettings<BaseTransactionsExecutorDbSettings>
    {
        /// <summary>
        /// Required.
        /// Name of the blockchain integration in CamelCase
        /// </summary>
        public string IntegrationName { get; set; }

        /// <summary>
        /// Optional.
        /// Provides options to access application settings.
        /// </summary>
        [CanBeNull]
        public Action<IReloadingManager<TAppSettings>> UseSettings { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="IAddressValidator"/> implementation factory. 
        /// </summary>
        public Func<ServiceFactoryContext<TAppSettings>, IAddressValidator> AddressValidatorFactory { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="IHealthProvider"/> implementation factory.
        /// </summary>
        public Func<ServiceFactoryContext<TAppSettings>, IHealthProvider> HealthProviderFactory { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="IIntegrationInfoService"/> implementation factory.
        /// </summary>
        public Func<ServiceFactoryContext<TAppSettings>, IIntegrationInfoService> IntegrationInfoServiceFactory { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="ITransactionEstimator"/> implementation factory.
        /// </summary>
        public Func<ServiceFactoryContext<TAppSettings>, ITransactionEstimator> TransactionEstimatorFactory { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="ITransactionExecutor"/> implementation factory.
        /// </summary>
        public Func<ServiceFactoryContext<TAppSettings>, ITransactionExecutor> TransactionExecutorFactory { get; set; }

        /// <summary>
        /// Optional, default implementation assumes that multiple address formats are not supported.
        /// <see cref="IAddressFormatsProvider"/> implementation factory.
        /// </summary>
        public Func<ServiceFactoryContext<TAppSettings>, IAddressFormatsProvider> AddressFormatsProviderFactory { get; set; }

        internal TransactionsExecutorServiceOptions()
        {
            AddressFormatsProviderFactory = c => new NotSupportedAddressFormatsProvider();
        }
    }
}

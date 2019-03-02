using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Repositories;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.Sdk.TransactionsExecutor.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

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
        public Action<IServiceCollection, IReloadingManager<TAppSettings>> UseSettings { get; set; }

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
        /// <see cref="ITransactionBroadcaster"/> implementation factory.
        /// </summary>
        public Func<ServiceFactoryContext<TAppSettings>, ITransactionBroadcaster> TransactionBroadcasterFactory { get; set; }

        /// <summary>
        /// Optional, default implementation assumes that multiple address formats are not supported.
        /// <see cref="IAddressFormatsProvider"/> implementation factory.
        /// </summary>
        public Func<ServiceFactoryContext<TAppSettings>, IAddressFormatsProvider> AddressFormatsProviderFactory { get; set; }

        /// <summary>
        /// Required only for "Transfer amounts" transactions model. Integration should either support “transfer coins”
        /// or “transfer amount” transactions model. Use <see cref="TransferCoinsTransactionsBuilderFactory"/> to support
        /// "transfer coins" transactions model.
        /// <see cref="ITransferAmountTransactionsBuilder"/> implementation factory.
        /// </summary>
        public Func<ServiceFactoryContext<TAppSettings>, ITransferAmountTransactionsBuilder> TransferAmountTransactionsBuilderFactory { get; set; }

        /// <summary>
        /// Required only for "Transfer coins" transactions model. Integration should either support “transfer coins”
        /// or “transfer amount” transactions model. Use <see cref="TransferAmountTransactionsBuilderFactory"/> to support
        /// "transfer amount" transactions model.
        /// <see cref="ITransferAmountTransactionsBuilder"/> implementation factory.
        /// </summary>
        public Func<ServiceFactoryContext<TAppSettings>, ITransferCoinsTransactionsBuilder> TransferCoinsTransactionsBuilderFactory { get; set; }

        /// <summary>
        /// Not Required.
        /// <see cref="IRawTransactionReadOnlyRepository"/> implementation factory.
        /// </summary>
        internal Func<string, ServiceFactoryContext<TAppSettings>, IRawTransactionReadOnlyRepository> RawTransactionReadOnlyRepositoryFactory { get; set; }

        /// <summary>
        /// Not Required.
        /// Used to disable logging in test scenarios
        /// </summary>
        internal bool DisableLogging { get; set; }

        internal TransactionsExecutorServiceOptions()
        {
            TransferAmountTransactionsBuilderFactory = c => new NotSupportedTransferAmountTransactionsBuilder();
            TransferCoinsTransactionsBuilderFactory = c => new NotSupportedTransferCoinsTransactionsBuilder();
            AddressFormatsProviderFactory = c => new NotSupportedAddressFormatsProvider();

            RawTransactionReadOnlyRepositoryFactory = (integrationName, context) =>
            {
                return RawTransactionReadOnlyRepository.Create(
                    integrationName.CamelToKebab(),
                    context.Settings.Nested(x => x.Db.AzureDataConnString));
            };
        }
    }
}

using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Controllers;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Repositories;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Settings;
using Lykke.Common.Log;
using Lykke.Sdk;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor
{
    [PublicAPI]
    public static class TransactionsExecutorServiceCollectionExtensions
    {
        /// <summary>
        /// Builds service provider for a blockchain integration transactions executor.
        /// </summary>
        public static IServiceProvider BuildBlockchainTransactionsExecutorServiceProvider<TAppSettings>(
            this IServiceCollection services,
            Action<TransactionsExecutorServiceOptions<TAppSettings>> configureOptions)

            where TAppSettings : class, ITransactionsExecutorSettings<BaseTransactionsExecutorDbSettings>
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var options = GetOptions(configureOptions);

            return services.BuildBlockchainIntegrationServiceProvider<TAppSettings>(integrationOptions =>
            {
                integrationOptions.ServiceName = $"{IntegrationNameTools.ToCamelCase(options.IntegrationName)} Transactions executor service";
                integrationOptions.UseSettings = settings =>
                {
                    RegisterCommonServices(services, settings, options);
                    RegisterImplementationServices(services, options, settings);

                    options.UseSettings?.Invoke(settings);
                };

                integrationOptions.LogsAzureTableName = $"{IntegrationNameTools.ToCamelCase(options.IntegrationName)}TransactionsExecutorLogs";
                integrationOptions.LogsAzureTableConnectionStringResolver = settings => settings.Db.LogsConnString;

                integrationOptions.AddController<IsAliveController>();
                integrationOptions.AddController<AddressesController>();
                integrationOptions.AddController<TransactionsController>();
                integrationOptions.AddController<IntegrationInfoController>();
            });
        }

        private static void RegisterCommonServices<TAppSettings>(
            IServiceCollection services, 
            IReloadingManager<TAppSettings> settings,
            TransactionsExecutorServiceOptions<TAppSettings> options)

            where TAppSettings : class, ITransactionsExecutorSettings<BaseTransactionsExecutorDbSettings>
        {
            services.AddTransient<IStartupManager, StartupManager>();

            services.AddSingleton<IHealthMonitor>(s => new HealthMonitor(
                s.GetRequiredService<ILogFactory>(),
                s.GetRequiredService<IHealthProvider>(),
                settings.CurrentValue.HealthMonitoringPeriod
            ));

            services.AddSingleton(s => RawTransactionReadOnlyRepository.Create(
                IntegrationNameTools.ToKebab(options.IntegrationName),
                settings.Nested(x => x.Db.AzureDataConnString)));
        }

        private static void RegisterImplementationServices<TAppSettings>(
            IServiceCollection services,
            TransactionsExecutorServiceOptions<TAppSettings> options, 
            IReloadingManager<TAppSettings> settings)

            where TAppSettings : class, ITransactionsExecutorSettings<BaseTransactionsExecutorDbSettings>
        {
            services.AddTransient(s => options.AddressValidatorFactory(new ServiceFactoryContext<TAppSettings>(s, settings)));
            services.AddTransient(s => options.HealthProviderFactory(new ServiceFactoryContext<TAppSettings>(s, settings)));
            services.AddTransient(s => options.IntegrationInfoServiceFactory(new ServiceFactoryContext<TAppSettings>(s, settings)));
            services.AddTransient(s => options.TransactionEstimatorFactory(new ServiceFactoryContext<TAppSettings>(s, settings)));
            services.AddTransient(s => options.TransactionExecutorFactory(new ServiceFactoryContext<TAppSettings>(s, settings)));
        }

        private static TransactionsExecutorServiceOptions<TAppSettings> GetOptions<TAppSettings>(
            Action<TransactionsExecutorServiceOptions<TAppSettings>> configureOptions)

            where TAppSettings : class, ITransactionsExecutorSettings<BaseTransactionsExecutorDbSettings>
        {
            var options = new TransactionsExecutorServiceOptions<TAppSettings>();

            configureOptions(options);

            if (string.IsNullOrWhiteSpace(options.IntegrationName))
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.IntegrationName)} is required.");
            }

            if (options.AddressValidatorFactory == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(options)}.{nameof(options.AddressValidatorFactory)} is required.");
            }

            if (options.HealthProviderFactory == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.HealthProviderFactory)} is required.");
            }

            if (options.IntegrationInfoServiceFactory == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(options)}.{nameof(options.IntegrationInfoServiceFactory)} is required.");
            }

            if (options.TransactionEstimatorFactory == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(options)}.{nameof(options.TransactionEstimatorFactory)} is required.");
            }

            if (options.TransactionExecutorFactory == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(options)}.{nameof(options.TransactionExecutorFactory)} is required.");
            }

            return options;
        }
    }
}

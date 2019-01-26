using System;
using JetBrains.Annotations;
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
            Action<TransactionsExecutorServiceOptions> configureOptions)

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

            var options = new TransactionsExecutorServiceOptions();

            configureOptions(options);

            if (string.IsNullOrWhiteSpace(options.IntegrationName))
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.IntegrationName)} is required.");
            }
            if (options.AddressValidatorFactory == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.AddressValidatorFactory)} is required.");
            }
            if (options.HealthProviderFactory == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.HealthProviderFactory)} is required.");
            }
            if (options.IntegrationInfoServiceFactory == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.IntegrationInfoServiceFactory)} is required.");
            }
            if (options.TransactionEstimatorFactory == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.TransactionEstimatorFactory)} is required.");
            }
            if (options.TransactionExecutorFactory == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.TransactionExecutorFactory)} is required.");
            }
            
            services.AddTransient(options.AddressValidatorFactory);
            services.AddTransient(options.HealthProviderFactory);
            services.AddTransient(options.IntegrationInfoServiceFactory);
            services.AddTransient(options.TransactionEstimatorFactory);
            services.AddTransient(options.TransactionExecutorFactory);

            services.AddTransient<IStartupManager, StartupManager>();

            return services.BuildBlockchainIntegrationServiceProvider<TAppSettings>(integrationOptions =>
            {
                integrationOptions.ServiceName = $"{options.IntegrationName} Sign service";
                integrationOptions.UseSettings = settings =>
                {
                    services.AddSingleton<IHealthMonitor>(s => new HealthMonitor(
                        s.GetRequiredService<ILogFactory>(),
                        s.GetRequiredService<IHealthProvider>(),
                        settings.CurrentValue.HealthMonitoringPeriod
                    ));
                    
                    services.AddSingleton(s => RawTransactionReadOnlyRepository.Create(
                        options.IntegrationName,
                        settings.Nested(x => x.Db.AzureDataConnString)));
                };

                integrationOptions.LogsAzureTableName = $"{options.IntegrationName}TransactionsExecutorLogs";
                integrationOptions.LogsAzureTableConnectionStringResolver = settings => settings.Db.LogsConnString;

                integrationOptions.AddController<IsAliveController>();
                integrationOptions.AddController<AddressesController>();
                integrationOptions.AddController<TransactionsController>();
                integrationOptions.AddController<IntegrationInfoController>();
            });
        }
    }
}

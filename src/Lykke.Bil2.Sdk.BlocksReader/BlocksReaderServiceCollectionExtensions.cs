﻿using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.RabbitMq;
using Lykke.Bil2.Sdk.BlocksReader.Repositories;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using Lykke.Bil2.Sdk.BlocksReader.Settings;
using Lykke.Common.Log;
using Lykke.Sdk;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Sdk.BlocksReader
{
    [PublicAPI]
    public static class BlocksReaderServiceCollectionExtensions
    {
        /// <summary>
        /// Builds service provider for a blockchain integration blocks reader.
        /// </summary>
        public static IServiceProvider BuildBlockchainBlocksReaderServiceProvider<TAppSettings>(
            this IServiceCollection services,
            Action<BlocksReaderServiceOptions<TAppSettings>> configureOptions)

            where TAppSettings : class, IBlocksReaderSettings<BaseBlocksReaderDbSettings, BaseBlocksReaderRabbitMqSettings>
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
                integrationOptions.ServiceName = $"{options.IntegrationName} Blocks reader";

                integrationOptions.UseSettings = settings =>
                {                  
                    RegisterCommonServices(services, options, settings);
                    RegisterIrreversibleBlocksRetrievingStrategy(services, options, settings);
                    RegisterImplementationServices(services, options, settings);
                    
                    options.UseSettings?.Invoke(services, settings);
                };

                if (options.DisableLogging)
                {
                    integrationOptions.DisableLogging();
                }

                integrationOptions.LogsAzureTableName = $"{options.IntegrationName}TransactionsExecutorLogs";
                integrationOptions.LogsAzureTableConnectionStringResolver = settings => settings.Db.LogsConnString;

                integrationOptions.AddDefaultIsAliveController();
            });
        }

        private static void RegisterCommonServices<TAppSettings>(
            IServiceCollection services,
            BlocksReaderServiceOptions<TAppSettings> options, 
            IReloadingManager<TAppSettings> settings) 
            
            where TAppSettings : IBlocksReaderSettings<BaseBlocksReaderDbSettings, BaseBlocksReaderRabbitMqSettings>
        {
            var currentSettings = settings.CurrentValue;

            services.AddTransient<IIrreversibleBlockListener, IrreversibleBlockListener>();
            services.AddTransient
            (
                s => new ReadBlockCommandsHandler
                (
                    s.GetRequiredService<IBlockReader>(),
                    s.GetRequiredService<IRawObjectWriteOnlyRepository>(),
                    currentSettings.RabbitMq.TransactionsBatchSize,
                    currentSettings.Db.MaxTransactionsSavingParallelism,
                    options.TransferModel
                )
            );
            services.AddTransient<IStartupManager, StartupManager>();

            services.AddSingleton
            (
                s => RawObjectWriteOnlyRepository.Create
                (
                    options.IntegrationName.CamelToKebab(),
                    settings.Nested(x => x.Db.AzureDataConnString)
                )
            );

            services.AddSingleton<IRabbitMqEndpoint>
            (
                s => new RabbitMqEndpoint
                (
                    s,
                    s.GetRequiredService<ILogFactory>(),
                    new Uri(settings.CurrentValue.RabbitMq.ConnString),
                    settings.CurrentValue.RabbitMq.Vhost == "/"
                        ? null
                        : settings.CurrentValue.RabbitMq.Vhost ?? options.RabbitVhost
                )
            );

            services.AddTransient<IRabbitMqConfigurator>
            (
                s => new RabbitMqConfigurator
                (
                    s.GetRequiredService<IRabbitMqEndpoint>(),
                    settings.CurrentValue.RabbitMq,
                    options.IntegrationName.CamelToKebab()
                )
            );
        }

        private static void RegisterImplementationServices<TAppSettings>(
            IServiceCollection services,
            BlocksReaderServiceOptions<TAppSettings> options, 
            IReloadingManager<TAppSettings> settings)
            
            where TAppSettings : IBlocksReaderSettings<BaseBlocksReaderDbSettings, BaseBlocksReaderRabbitMqSettings>
        {
            services.AddTransient(s => options.BlockReaderFactory(new ServiceFactoryContext<TAppSettings>(s, settings)));
        }

        private static void RegisterIrreversibleBlocksRetrievingStrategy<TAppSettings>(
            IServiceCollection services,
            BlocksReaderServiceOptions<TAppSettings> options,
            IReloadingManager<TAppSettings> settings)

            where TAppSettings : IBlocksReaderSettings<BaseBlocksReaderDbSettings, BaseBlocksReaderRabbitMqSettings>
        {
            var kebabIntegrationName = options.IntegrationName.CamelToKebab();
            var eventsExchangeName = RabbitMqExchangeNamesFactory.GetIntegrationEventsExchangeName(kebabIntegrationName);

            switch (options.IrreversibleBlockRetrievingStrategy)
            {
                case IrreversibleBlockRetrievingStrategy.NotSupported:
                    break;

                case IrreversibleBlockRetrievingStrategy.Pulling:
                    services.AddSingleton<IIrreversibleBlockMonitor>(s =>
                        new IrreversibleBlockMonitor(
                            s.GetRequiredService<ILogFactory>(),
                            s.GetRequiredService<IIrreversibleBlockProvider>(),
                            s.GetRequiredService<IIrreversibleBlockListener>(),
                            settings.CurrentValue.LastIrreversibleBlockMonitoringPeriod));
                    services.AddTransient<IIrreversibleBlockListener>(s =>
                        new IrreversibleBlockListener(
                            () => s.GetRequiredService<IRabbitMqEndpoint>().CreatePublisher(eventsExchangeName)));
                    services.AddTransient(s =>
                        options.IrreversibleBlockProviderFactory(new ServiceFactoryContext<TAppSettings>(s, settings)));
                    break;

                case IrreversibleBlockRetrievingStrategy.Pushing:
                    services.AddTransient<IIrreversibleBlockListener>(s =>
                        new IrreversibleBlockListener(
                            () => s.GetRequiredService<IRabbitMqEndpoint>().CreatePublisher(eventsExchangeName)));
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(options.IrreversibleBlockRetrievingStrategy), options.IrreversibleBlockRetrievingStrategy, "Unknown strategy");
            }
        }

        private static BlocksReaderServiceOptions<TAppSettings> GetOptions<TAppSettings>(
            Action<BlocksReaderServiceOptions<TAppSettings>> configureOptions)

            where TAppSettings : IBlocksReaderSettings<BaseBlocksReaderDbSettings, BaseBlocksReaderRabbitMqSettings>
        {
            var options = new BlocksReaderServiceOptions<TAppSettings>();

            configureOptions(options);

            if (string.IsNullOrWhiteSpace(options.IntegrationName))
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.IntegrationName)} is required.");
            }

            if (options.BlockReaderFactory == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.BlockReaderFactory)} is required.");
            }

            if (!options.IsTransferModelSet)
            {
                throw new InvalidOperationException($"Transfer model should be set. Invoke either {nameof(BlocksReaderServiceOptions<TAppSettings>.UseTransferAmountTransactionsModel)} or {nameof(BlocksReaderServiceOptions<TAppSettings>.UseTransferCoinsTransactionsModel)}.");
            }

            return options;
        }
    }
}

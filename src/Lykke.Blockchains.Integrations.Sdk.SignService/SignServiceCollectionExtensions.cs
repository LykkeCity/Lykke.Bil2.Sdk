using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.Sdk.SignService.Controllers;
using Lykke.Blockchains.Integrations.Sdk.SignService.Models;
using Lykke.Blockchains.Integrations.Sdk.SignService.Services;
using Lykke.Blockchains.Integrations.Sdk.SignService.Settings;
using Lykke.Sdk;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Blockchains.Integrations.Sdk.SignService
{
    [PublicAPI]
    public static class SignServiceCollectionExtensions
    {
        /// <summary>
        /// Builds service provider for a blockchain integration sign service.
        /// </summary>
        public static IServiceProvider BuildBlockchainSignServiceProvider<TAppSettings>(
            this IServiceCollection services,
            Action<SignServiceOptions<TAppSettings>> configureOptions)

            where TAppSettings : BaseSignServiceSettings
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
                integrationOptions.ServiceName = $"{options.IntegrationName} Sign service";
                integrationOptions.UseSettings = settings =>
                {
                    RegisterCommonServices(services, settings);
                    RegisterImplementationServices(services, options, settings);

                    options.UseSettings?.Invoke(settings);
                };

                integrationOptions.DisableLogging();
                
                integrationOptions.AddDefaultIsAliveController();
                integrationOptions.AddController<AddressesController>();
                integrationOptions.AddController<TransactionsController>();
            });
        }

        private static void RegisterCommonServices<TAppSettings>(
            IServiceCollection services, 
            IReloadingManager<TAppSettings> settings)

            where TAppSettings : BaseSignServiceSettings
        {
            services.AddTransient<IStartupManager, StartupManager>();
            services.AddSingleton(new EncryptionConfiguration(new Base58String(settings.CurrentValue.EncryptionPrivateKey)));
        }

        private static void RegisterImplementationServices<TAppSettings>(
            IServiceCollection services,
            SignServiceOptions<TAppSettings> options, 
            IReloadingManager<TAppSettings> settings) 
            
            where TAppSettings : BaseSignServiceSettings
        {
            services.AddTransient(s => options.AddressGeneratorFactory(new ServiceFactoryContext<TAppSettings>(s, settings)));
            services.AddTransient(s => options.TransactionSignerFactory(new ServiceFactoryContext<TAppSettings>(s, settings)));
        }

        private static SignServiceOptions<TAppSettings> GetOptions<TAppSettings>(Action<SignServiceOptions<TAppSettings>> configureOptions)
            where TAppSettings : BaseSignServiceSettings
        {
            var options = new SignServiceOptions<TAppSettings>();

            configureOptions(options);

            if (string.IsNullOrWhiteSpace(options.IntegrationName))
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.IntegrationName)} is required.");
            }
            if (options.TransactionSignerFactory == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.TransactionSignerFactory)} is required.");
            }
            if (options.AddressGeneratorFactory == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.AddressGeneratorFactory)} is required.");
            }

            return options;
        }
    }
}

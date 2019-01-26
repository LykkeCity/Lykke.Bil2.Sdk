using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.Sdk.SignService.Controllers;
using Lykke.Blockchains.Integrations.Sdk.SignService.Models;
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

            var options = new SignServiceOptions<TAppSettings>();

            configureOptions(options);

            if (options.TransactionSignerFactory == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.TransactionSignerFactory)} is required.");
            }
            if (options.AddressGeneratorFactory == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.AddressGeneratorFactory)} is required.");
            }
            
            return services.BuildBlockchainIntegrationServiceProvider<TAppSettings>(integrationOptions =>
            {
                integrationOptions.ServiceName = $"{options.IntegrationName} Sign service";
                integrationOptions.UseSettings = settings =>
                {
                    var privateKey = Base58String.Create(settings.CurrentValue.EncryptionPrivateKey);

                    services.AddSingleton(new EncryptionConfiguration(privateKey));

                    services.AddTransient(s => options.AddressGeneratorFactory(new ServiceFactoryContext<TAppSettings>(s, settings)));
                    services.AddTransient(s => options.TransactionSignerFactory(new ServiceFactoryContext<TAppSettings>(s, settings)));

                    options.UseSettings?.Invoke(settings);
                };
                integrationOptions.DisableLogging();
                integrationOptions.AddDefaultIsAliveController();
                integrationOptions.AddController<AddressesController>();
                integrationOptions.AddController<TransactionsController>();
            });
        }
    }
}

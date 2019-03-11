using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Sdk.AspNetCore;
using Lykke.Bil2.Sdk.Services;
using Lykke.Logs.Loggers.LykkeSlack;
using Lykke.Numerics.Money;
using Lykke.Sdk;
using Lykke.Sdk.Settings;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Lykke.Bil2.Sdk
{
    [PublicAPI]
    public static class BlockchainIntegrationServiceCollectionExtensions
    {
        /// <summary>
        /// Build service provider for a blockchain integration service.
        /// </summary>
        public static IServiceProvider BuildBlockchainIntegrationServiceProvider<TAppSettings>(
            this IServiceCollection services,
            Action<BlockchainIntegrationServiceOptions<TAppSettings>> configureOptions)

            where TAppSettings : class, IAppSettings
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            var options = new BlockchainIntegrationServiceOptions<TAppSettings>();

            configureOptions.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.ServiceName))
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.ServiceName)} is required.");
            }
            if (options.ControllerTypes == null)
            {
                throw new InvalidOperationException($"{nameof(options)}.{nameof(options.ControllerTypes)} is required.");
            }

            return services.BuildServiceProvider<TAppSettings>(lykkeSdkOptions =>
            {
                lykkeSdkOptions.ConfigureApplicationParts = partsManager =>
                {
                    ConfigurePartsManager(options, partsManager);
                };

                lykkeSdkOptions.DisableFluentValidation();
                lykkeSdkOptions.DisableValidationFilter();

                if (options.UseSettings != null)
                {
                    lykkeSdkOptions.Extend = (services1, settings) =>
                    {
                        services.AddSingleton<ISettingsRenderer>(new SettingsRenderer<TAppSettings>(settings));

                        options.UseSettings.Invoke(settings);
                    };
                }

                lykkeSdkOptions.SwaggerOptions = new LykkeSwaggerOptions
                {
                    ApiTitle = $"{options.ServiceName} API",
                    ApiVersion = "v1"
                };

                lykkeSdkOptions.Swagger = swaggerOptions =>
                {
                    swaggerOptions.MapType<Version>(() => new Schema{ Type = "string", Format = "version"});
                    swaggerOptions.MapType<Base58String>(() => new Schema{ Type = "string", Format = "byte"});
                    swaggerOptions.MapType<EncryptedString>(() => new Schema{ Type = "string", Format = "byte"});
                    swaggerOptions.MapType<UMoney>(() => new Schema{ Type = "string", Format = "coinsAmount"});
                    swaggerOptions.MapType<Money>(() => new Schema{ Type = "string", Format = "coinsChange"});
                    swaggerOptions.MapType<AssetId>(() => new Schema{ Type = "string", Format = "assetId"});
                    swaggerOptions.MapType<Address>(() => new Schema{ Type = "string", Format = "address"});
                    swaggerOptions.MapType<AddressTag>(() => new Schema{ Type = "string", Format = "addressTag"});
                };

                lykkeSdkOptions.Logs = logsOptions =>
                {
                    ConfigureLogs(options, logsOptions, lykkeSdkOptions);
                };
            });
        }

        private static void ConfigureLogs<TAppSettings>(BlockchainIntegrationServiceOptions<TAppSettings> options,
            LykkeLoggingOptions<TAppSettings> logsOptions, LykkeServiceOptions<TAppSettings> lykkeSdkOptions)
            
            where TAppSettings : IAppSettings
        {
            if (options.HaveToDisableLogging)
            {
                logsOptions.UseEmptyLogging();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(options.LogsAzureTableName))
                {
                    throw new InvalidOperationException(
                        $"{nameof(options)}.{nameof(options.LogsAzureTableName)} is required when logging is not disabled.");
                }

                if (options.LogsAzureTableConnectionStringResolver == null)
                {
                    throw new InvalidOperationException(
                        $"{nameof(options)}.{nameof(options.LogsAzureTableConnectionStringResolver)} is required when logging is not disabled.");
                }

                logsOptions.AzureTableName = options.LogsAzureTableName;
                logsOptions.AzureTableConnectionStringResolver = options.LogsAzureTableConnectionStringResolver;

                logsOptions.Extended = extendedLogs =>
                {
                    extendedLogs.AddAdditionalSlackChannel("BlockChainIntegration", channelOptions =>
                    {
                        channelOptions.MinLogLevel = Microsoft.Extensions.Logging.LogLevel.Information;
                        channelOptions.SpamGuard.DisableGuarding();
                        channelOptions.IncludeHealthNotifications();
                    });

                    extendedLogs.AddAdditionalSlackChannel("BlockChainIntegrationImportantMessages",
                        channelOptions =>
                        {
                            channelOptions.MinLogLevel = Microsoft.Extensions.Logging.LogLevel.Warning;
                            channelOptions.SpamGuard.DisableGuarding();
                            channelOptions.IncludeHealthNotifications();
                        });
                };
            }

            options.Extended?.Invoke(lykkeSdkOptions);
        }

        private static void ConfigurePartsManager<TAppSettings>(
            BlockchainIntegrationServiceOptions<TAppSettings> options,
            ApplicationPartManager partsManager) 
            
            where TAppSettings : IAppSettings
        {
            // Removes SDK assemblies with controllers to prevent they implicit adding to the app.
            var blockchainSdkAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var lykkeSdkAssemblyName = typeof(Lykke.Sdk.Controllers.IsAliveController).Assembly.GetName().Name;
            var explicitControllerAssemblyNames = options.ControllerTypes
                .Select(t => t.Assembly.GetName().Name)
                .ToHashSet();
            var partsToRemove = partsManager.ApplicationParts
                .Where(part =>
                    part.Name == blockchainSdkAssemblyName ||
                    part.Name == lykkeSdkAssemblyName ||
                    explicitControllerAssemblyNames.Contains(part.Name))
                .ToArray();

            foreach (var part in partsToRemove)
            {
                partsManager.ApplicationParts.Remove(part);
            }

            var featureProvider = new ExplicitControllersFeatureProvider(options.ControllerTypes);
            partsManager.FeatureProviders.Add(featureProvider);
        }
    }
}

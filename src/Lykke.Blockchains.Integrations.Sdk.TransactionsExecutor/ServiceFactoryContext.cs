using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Settings;
using Lykke.SettingsReader;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor
{
    [PublicAPI]
    public class ServiceFactoryContext<TAppSettings>

        where TAppSettings : class, ITransactionsExecutorSettings<BaseTransactionsExecutorDbSettings>
    {
        public IServiceProvider Services { get; }

        public IReloadingManager<TAppSettings> Settings { get; }

        public ServiceFactoryContext(IServiceProvider services, IReloadingManager<TAppSettings> settings)
        {
            Services = services;
            Settings = settings;
        }
    }
}

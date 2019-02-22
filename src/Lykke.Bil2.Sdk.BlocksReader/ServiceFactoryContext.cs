using System;
using JetBrains.Annotations;
using Lykke.Bil2.Sdk.BlocksReader.Settings;
using Lykke.SettingsReader;

namespace Lykke.Bil2.Sdk.BlocksReader
{
    /// <summary>
    /// Service factory context
    /// </summary>
    [PublicAPI]
    public class ServiceFactoryContext<TAppSettings>

        where TAppSettings : IBlocksReaderSettings<BaseBlocksReaderDbSettings>
    {
        /// <summary>
        /// Service provider
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Application setting
        /// </summary>
        public IReloadingManager<TAppSettings> Settings { get; }

        /// <summary>
        /// Service factory context
        /// </summary>
        public ServiceFactoryContext(IServiceProvider services, IReloadingManager<TAppSettings> settings)
        {
            Services = services;
            Settings = settings;
        }
    }
}

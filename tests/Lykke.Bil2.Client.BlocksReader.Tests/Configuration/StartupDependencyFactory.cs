using System;
using Lykke.Bil2.Sdk.BlocksReader;
using Lykke.Bil2.Sdk.BlocksReader.Settings;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Configuration
{
    public interface IStartupDependencyFactory
    {
        Action<BlocksReaderServiceOptions<TAppSettings>> GetOptionsConfiguration<TAppSettings>()
            where TAppSettings : BaseBlocksReaderSettings<DbSettings, RabbitMqSettings>;

        IServiceProvider ServerServiceProvider { get; set; }
    }

    //This class provides dependency configuration for application hosted in test server
    public class StartupDependencyFactorySingleton
    {
        public static IStartupDependencyFactory Instance { get; set; }

        private StartupDependencyFactorySingleton()
        {
        }
    }

    public class StartupDependencyFactory<TAppSettings> : IStartupDependencyFactory 
        where TAppSettings : BaseBlocksReaderSettings<DbSettings, RabbitMqSettings>
    {
        private readonly Action<BlocksReaderServiceOptions<TAppSettings>> _registerAction;

        internal StartupDependencyFactory(Action<BlocksReaderServiceOptions<TAppSettings>> registerAction)
        {
            _registerAction = registerAction;
        }

        public Action<BlocksReaderServiceOptions<TSettings>> GetOptionsConfiguration<TSettings>() 
            where TSettings : BaseBlocksReaderSettings<DbSettings, RabbitMqSettings>
        {
            return (Action<BlocksReaderServiceOptions<TSettings>>)_registerAction;
        }

        public IServiceProvider ServerServiceProvider { get; set; }
    }
}

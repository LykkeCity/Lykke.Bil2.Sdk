using System;
using Lykke.Bil2.Sdk.SignService;
using Lykke.Bil2.Sdk.SignService.Settings;

namespace Lykke.Bil2.SignService.Client.Tests.Configuration
{
    public interface IStartupDependencyFactory
    {
        Action<SignServiceOptions<TAppSettings>> GetOptionsConfiguration<TAppSettings>() where TAppSettings : BaseSignServiceSettings;
    }

    //This class provides dependency configuration for application hosted in test server
    public class StartupDependencyFactorySingleton
    {
        private static object _lock = new object();
        private static IStartupDependencyFactory _instance;
        public static IStartupDependencyFactory Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                lock (_lock)
                {
                    _instance = value;
                }
            }
        }

        private StartupDependencyFactorySingleton()
        {
        }
    }

    public class StartupDependencyFactory<TAppSettings> : IStartupDependencyFactory where TAppSettings : BaseSignServiceSettings
    {
        private readonly Action<SignServiceOptions<TAppSettings>> _registerAction;

        internal StartupDependencyFactory(Action<SignServiceOptions<TAppSettings>> registerAction)
        {
            _registerAction = registerAction;
        }

        public Action<SignServiceOptions<TSettings>> GetOptionsConfiguration<TSettings>() where TSettings : BaseSignServiceSettings
        {
            return (Action<SignServiceOptions<TSettings>>)_registerAction;
        }
    }
}

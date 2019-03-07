using Lykke.Bil2.Sdk.TransactionsExecutor;
using Lykke.Bil2.Sdk.TransactionsExecutor.Settings;
using System;

namespace Lykke.Bil2.Client.TransactionExecutor.Tests.Configuration
{
    public interface IStartupDependencyFactory
    {
        Action<TransactionsExecutorServiceOptions<TAppSettings>> GetOptionsConfiguration<TAppSettings>() where TAppSettings : BaseTransactionsExecutorSettings<DbSettings>;
    }

    //This class provides dependency configuration for application hosted in test server
    public class StartupDependencyFactorySingleton
    {
        public static IStartupDependencyFactory Instance { get; set; }

        private StartupDependencyFactorySingleton()
        {
        }
    }

    public class StartupDependencyFactory<TAppSettings> : IStartupDependencyFactory where TAppSettings : BaseTransactionsExecutorSettings<DbSettings>
    {
        private readonly Action<TransactionsExecutorServiceOptions<TAppSettings>> _registerAction;

        internal StartupDependencyFactory(Action<TransactionsExecutorServiceOptions<TAppSettings>> registerAction)
        {
            _registerAction = registerAction;
        }

        public Action<TransactionsExecutorServiceOptions<TSettings>> GetOptionsConfiguration<TSettings>() where TSettings : BaseTransactionsExecutorSettings<DbSettings>
        {
            return (Action<TransactionsExecutorServiceOptions<TSettings>>)_registerAction;
        }
    }
}

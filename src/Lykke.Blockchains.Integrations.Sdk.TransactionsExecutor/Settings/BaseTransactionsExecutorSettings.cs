using System;
using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Settings
{
    /// <summary>
    /// Base settings for a transactions executor application.
    /// </summary>
    /// <typeparam name="TDbSettings">Database settings type.</typeparam>
    [PublicAPI]
    public class BaseTransactionsExecutorSettings<TDbSettings> : 
        BaseAppSettings,
        ITransactionsExecutorSettings<TDbSettings> 
        
        where TDbSettings : BaseTransactionsExecutorDbSettings
    {
        /// <inheritdoc />
        public TDbSettings Db { get; set; }

        /// <inheritdoc />
        public TimeSpan HealthMonitoringPeriod { get; set; }
    }
}

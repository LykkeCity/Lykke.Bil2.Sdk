using System;
using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Settings
{
    /// <summary>
    /// Settings abstraction for a transactions executor application.
    /// </summary>
    /// <typeparam name="TDbSettings">Database settings type.</typeparam>
    [PublicAPI]
    public interface ITransactionsExecutorSettings<out TDbSettings> : IAppSettings

        where TDbSettings : BaseTransactionsExecutorDbSettings
    {
        /// <summary>
        /// Database settings.
        /// </summary>
        TDbSettings Db { get; }

        /// <summary>
        /// Period of integration health monitoring.
        /// </summary>
        TimeSpan HealthMonitoringPeriod { get; }
    }
}

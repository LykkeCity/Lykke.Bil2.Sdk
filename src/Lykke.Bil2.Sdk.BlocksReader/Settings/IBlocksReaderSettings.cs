using System;
using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Bil2.Sdk.BlocksReader.Settings
{
    /// <summary>
    /// Settings abstraction for a blocks reader application.
    /// </summary>
    /// <typeparam name="TDbSettings">Database settings type.</typeparam>
    /// <typeparam name="TRabbitMqSettings">RabbitMq settings type.</typeparam>
    [PublicAPI]
    public interface IBlocksReaderSettings<out TDbSettings, out TRabbitMqSettings> : IAppSettings

        where TDbSettings : BaseBlocksReaderDbSettings
        where TRabbitMqSettings : BaseBlocksReaderRabbitMqSettings
    {
        /// <summary>
        /// Database settings.
        /// </summary>
        TDbSettings Db { get; }

        /// <summary>
        /// RabbitMq settings.
        /// </summary>
        TRabbitMqSettings RabbitMq { get; }

        /// <summary>
        /// Monitoring period of the last irreversible block.
        /// </summary>
        /// <remarks>
        /// Used only if blockchain has irreversible blocks and they can be only pulled from the blockchain.
        /// </remarks>
        [Optional]
        TimeSpan LastIrreversibleBlockMonitoringPeriod { get; }
    }
}

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
    [PublicAPI]
    public interface IBlocksReaderSettings<out TDbSettings> : IAppSettings

        where TDbSettings : BaseBlocksReaderDbSettings
    {
        /// <summary>
        /// Database settings.
        /// </summary>
        TDbSettings Db { get; }

        /// <summary>
        /// RabbitMq connection string.
        /// </summary>
        [AmqpCheck]
        string RabbitConnString { get; }

        /// <summary>
        /// Monitoring period of the last irreversible block.
        /// </summary>
        /// <remarks>
        /// Used only if blockchain has irreversible blocks and they can be only pulled from the blockchain.
        /// </remarks>
        [Optional]
        TimeSpan LastIrreversibleBlockMonitoringPeriod { get; }

        /// <summary>
        /// Number of the threads used to listen messages from the RabbitMq.
        /// </summary>
        int MessageListeningParallelism { get; }
    }
}

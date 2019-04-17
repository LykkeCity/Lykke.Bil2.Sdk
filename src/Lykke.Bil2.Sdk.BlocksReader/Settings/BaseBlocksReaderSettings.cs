using System;
using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Bil2.Sdk.BlocksReader.Settings
{
    /// <summary>
    /// Base settings for a blocks reader application.
    /// </summary>
    [PublicAPI]
    public class BaseBlocksReaderSettings<TDbSettings, TRabbitMqSettings> : 
        BaseAppSettings,
        IBlocksReaderSettings<TDbSettings, TRabbitMqSettings> 
        
        where TDbSettings : BaseBlocksReaderDbSettings 
        where TRabbitMqSettings : BaseBlocksReaderRabbitMqSettings
    {
        /// <inheritdoc />
        public TDbSettings Db { get; set; }

        /// <inheritdoc />
        public TRabbitMqSettings RabbitMq { get; set; }

        /// <inheritdoc />
        [Optional]
        public string RabbitVhost { get; set; }

        /// <inheritdoc />
        [Optional]
        public TimeSpan LastIrreversibleBlockMonitoringPeriod { get; set; }
    }
}

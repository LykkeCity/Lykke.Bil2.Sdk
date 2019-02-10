﻿using System;
using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader.Settings
{
    /// <summary>
    /// Base settings for a blocks reader application.
    /// </summary>
    [PublicAPI]
    public class BaseBlocksReaderSettings<TDbSettings> : 
        BaseAppSettings,
        IBlocksReaderSettings<TDbSettings> 
        
        where TDbSettings : BaseBlocksReaderDbSettings
    {
        /// <inheritdoc />
        public TDbSettings Db { get; set; }

        /// <inheritdoc />
        [AmqpCheck]
        public string RabbitConnStrng { get; set; }

        /// <inheritdoc />
        [Optional]
        public TimeSpan LastIrreversibleBlockMonitoringPeriod { get; set; }

        /// <inheritdoc />
        public int MessageListeningParallelism { get; set; }
    }
}

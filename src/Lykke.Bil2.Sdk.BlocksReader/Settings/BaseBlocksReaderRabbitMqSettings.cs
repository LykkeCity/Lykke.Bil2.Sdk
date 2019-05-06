using System;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Bil2.Sdk.BlocksReader.Settings
{
    /// <summary>
    /// Base RabbitMq settings of a blocks reader application.
    /// </summary>
    [PublicAPI]
    public class BaseBlocksReaderRabbitMqSettings
    {
        /// <summary>
        /// RabbitMq connection string.
        /// </summary>
        [AmqpCheck]
        public string ConnString { get; set; }

        /// <summary>
        /// Transactions count to send in the single batch event.
        /// </summary>
        public int TransactionsBatchSize { get; set; }

        /// <summary>
        /// RabbitMq Vhost name.
        /// </summary>
        [Optional]
        public string Vhost { get; set; }

        /// <summary>
        /// Number of the threads used to listen messages from the RabbitMq.
        /// </summary>
        [Optional]
        public int MessageConsumersCount { get; set; } = 4;

        /// <summary>
        /// Number of the threads used to process messages from the RabbitMq.
        /// </summary>
        [Optional]
        public int MessageProcessorsCount { get; set; } = 8;

        /// <summary>
        /// Default first level retry timeout.
        /// </summary>
        [Optional]
        public TimeSpan DefaultFirstLevelRetryTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Max age of the message to retry it at the first level (includes all attempts).
        /// </summary>
        [Optional]
        public TimeSpan MaxFirstLevelRetryMessageAge { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Max count of the first level retries for a message.
        /// </summary>
        [Optional]
        public int MaxFirstLevelRetryCount { get; set; } = 20;

        /// <summary>
        /// Max messages which can be retried at the first level retries at the moment.
        /// </summary>
        [Optional]
        public int FirstLevelRetryQueueCapacity { get; set; } = 1000;

        /// <summary>
        /// Max message which can wait for the free processor right after the read by a consumer.
        /// </summary>
        [Optional]
        public int ProcessingQueueCapacity { get; set; } = 500;
    }
}

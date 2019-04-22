using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Bil2.RabbitMq.Subscription;

namespace Lykke.Bil2.Client.BlocksReader.Options
{
    /// <summary>
    /// Options for a blockchain integration blocks reader client.
    /// </summary>
    [PublicAPI]
    public class BlocksReaderClientOptions
    {
        /// <summary>
        /// Required.
        /// RabbitMq connection string to communicate with blockchain integration blocks reader.
        /// </summary>
        public string RabbitMqConnString { get; set; }

        /// <summary>
        /// Optional.
        /// Default value is 30 seconds.
        /// Default first level retry timeout.
        /// </summary>
        public TimeSpan DefaultFirstLevelRetryTimeout { get; set; }
        
        /// <summary>
        /// Optional.
        /// Default value is 10 minutes.
        /// Max age of the message to retry it at the first level (includes all attempts).
        /// </summary>
        public TimeSpan MaxFirstLevelRetryMessageAge { get; set; }
        
        /// <summary>
        /// Optional.
        /// Default value is 50.
        /// Max count of the first level retries for a message.
        /// </summary>
        public int MaxFirstLevelRetryCount { get; set; }
        
        /// <summary>
        /// Optional.
        /// Default value is 10000.
        /// Max messages which can be retried at the first level retries at the moment.
        /// </summary>
        public int FirstLevelRetryQueueCapacity { get; set; }
        
        /// <summary>
        /// Optional.
        /// Default value is 1000.
        /// Max message which can wait for the free processor right after the read by a consumer.
        /// </summary>
        public int ProcessingQueueCapacity { get; set; }
        
        /// <summary>
        /// Optional.
        /// Default value is 4.
        /// Number of the threads used to listen messages from the RabbitMq.
        /// </summary>
        public int MessageConsumersCount { get; set; }
        
        /// <summary>
        /// Optional.
        /// Default value is 8.
        /// Number of the threads used to process messages from the RabbitMq.
        /// </summary>
        public int MessageProcessorsCount { get; set; }

        /// <summary>
        /// Required.
        /// <see cref="IBlockEventsHandler"/> implementation factory. 
        /// </summary>
        public Func<IServiceProvider, IBlockEventsHandler> BlockEventsHandlerFactory { get; set; }

        /// <summary>
        /// RabbitMQ Vhost
        /// </summary>
        public string RabbitVhost { get; set; }

        internal ICollection<Func<IServiceProvider, IMessageFilter>> MessageFilters { get; }
        
        internal IReadOnlyCollection<string> IntegrationNames => _integrationNames;
        
        private List<string> _integrationNames;

        internal BlocksReaderClientOptions()
        {
            _integrationNames = new List<string>();
            DefaultFirstLevelRetryTimeout = TimeSpan.FromSeconds(30);
            MaxFirstLevelRetryMessageAge = TimeSpan.FromMinutes(10);
            MaxFirstLevelRetryCount = 50;
            FirstLevelRetryQueueCapacity = 10000;
            ProcessingQueueCapacity = 1000;
            MessageConsumersCount = 4;
            MessageProcessorsCount = 8;
            MessageFilters = new List<Func<IServiceProvider, IMessageFilter>>();
        }

        /// <summary>
        /// Adds blockchain integration.
        /// </summary>
        /// <param name="integrationName"></param>
        public void AddIntegration(string integrationName)
        {
            if (string.IsNullOrWhiteSpace(integrationName))
            {
                throw new ArgumentException("Should be not empty string", nameof(integrationName));
            }

            _integrationNames.Add(integrationName);
        }

        /// <summary>
        /// Adds client-wide message filter
        /// </summary>
        /// <param name="factory"></param>
        public void AddMessageFilter(Func<IServiceProvider, IMessageFilter> factory)
        {
            MessageFilters.Add(factory);
        }
    }
}

using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Client.BlocksReader
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
        /// Required.
        /// Parallelism of the RabbitMq message listening for each blockchain integration.
        /// </summary>
        public int MessageListeningParallelism { get; set; }

        internal IReadOnlyCollection<string> IntegrationNames => _integrationNames;
        
        private List<string> _integrationNames;

        internal BlocksReaderClientOptions()
        {
            _integrationNames = new List<string>();
            MessageListeningParallelism = 8;
        }

        /// <summary>
        /// Adds blockchain integration.
        /// </summary>
        /// <param name="integrationName"></param>
        public void AddIntegration(string integrationName)
        {
            _integrationNames.Add(integrationName);
        }
    }
}

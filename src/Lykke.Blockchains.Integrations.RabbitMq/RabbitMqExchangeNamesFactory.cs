using System;
using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.RabbitMq
{
    /// <summary>
    /// Exchange names factory
    /// </summary>
    [PublicAPI]
    public static class RabbitMqExchangeNamesFactory
    {
        /// <summary>
        /// Returns name of the blockchain integration commands exchange
        /// </summary>
        /// <param name="integrationName">Name of the blockchain integration in the kebab-notation</param>
        public static string GetIntegrationCommandsExchangeName(string integrationName)
        {
            if (string.IsNullOrWhiteSpace(integrationName))
            {
                throw new ArgumentException("Should be not empty string", nameof(integrationName));
            }

            return $"bil-v2.bcn-{integrationName}.commands";
        }

        /// <summary>
        /// Returns name of the blockchain integration events exchange
        /// </summary>
        /// <param name="integrationName">Name of the blockchain integration in the kebab-notation</param>
        public static string GetIntegrationEventsExchangeName(string integrationName)
        {
            if (string.IsNullOrWhiteSpace(integrationName))
            {
                throw new ArgumentException("Should be not empty string", nameof(integrationName));
            }

            return $"bil-v2.bcn-{integrationName}.events";
        }
    }
}

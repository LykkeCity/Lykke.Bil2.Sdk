using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader
{
    /// <summary>
    /// Options of the blockchain integration blocks reader application.
    /// </summary>
    [PublicAPI]
    public class BlocksReaderApplicationOptions
    {
        /// <summary>
        /// Required.
        /// Name of the blockchain integration in CamelCase
        /// </summary>
        public string IntegrationName { get; set; }
    }
}

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
        /// Name of the blockchain integration. Words should be separated by the space, each word should be started from the capital case.
        /// </summary>
        public string IntegrationName { get; set; }
    }
}

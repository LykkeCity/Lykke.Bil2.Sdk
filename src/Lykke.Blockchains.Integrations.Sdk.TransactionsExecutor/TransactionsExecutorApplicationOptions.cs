using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor
{
    /// <summary>
    /// Options of the blockchain integration transactions executor application.
    /// </summary>
    [PublicAPI]
    public class TransactionsExecutorApplicationOptions
    {
        /// <summary>
        /// Required.
        /// Name of the blockchain integration. Words should be separated by the space, each word should be started from the capital case.
        /// </summary>
        public string IntegrationName { get; set; }
    }
}

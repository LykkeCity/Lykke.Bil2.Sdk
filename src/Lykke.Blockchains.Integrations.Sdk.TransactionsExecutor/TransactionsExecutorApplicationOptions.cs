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
        /// Name of the integration.
        /// </summary>
        public string IntegrationName { get; set; }
    }
}

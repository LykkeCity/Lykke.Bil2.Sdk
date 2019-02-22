using JetBrains.Annotations;

namespace Lykke.Bil2.Sdk.TransactionsExecutor
{
    /// <summary>
    /// Options of the blockchain integration transactions executor application.
    /// </summary>
    [PublicAPI]
    public class TransactionsExecutorApplicationOptions
    {
        /// <summary>
        /// Required.
        /// Name of the blockchain integration in CamelCase
        /// </summary>
        public string IntegrationName { get; set; }
    }
}

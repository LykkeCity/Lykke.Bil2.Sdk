using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/sending/built
    /// </summary>
    [PublicAPI]
    public class BuildSendingTransactionResponse
    {
        /// <summary>
        /// Implementation specific transaction context. 
        /// </summary>
        [JsonProperty("transactionContext")]
        public Base64String TransactionContext { get; set; }
    }
}

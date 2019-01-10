using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Endpoint: [GET] /api/transactions/{transactionHash}/raw
    /// </summary>
    [PublicAPI]
    public class GetRawTransactionResponse
    {
        /// <summary>
        /// Raw transaction.
        /// </summary>
        [JsonProperty("raw")]
        public Base64String Raw { get; set; }
    }
}
using System.Collections.Generic;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/sending/estimated
    /// </summary>
    public class EstimateSendingTransactionResponse
    {
        /// <summary>
        /// Estimated transaction fee for particular asset ID.
        /// </summary>
        [JsonProperty("assetEstimatedFee")]
        public IDictionary<string, CoinsAmount> AssetEstimatedFee { get; set; }
    }
}

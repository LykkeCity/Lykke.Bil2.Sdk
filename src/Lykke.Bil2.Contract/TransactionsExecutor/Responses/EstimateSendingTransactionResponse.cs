using System;
using System.Collections.Generic;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Responses
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
        public IDictionary<AssetId, CoinsAmount> AssetEstimatedFee { get; }

        /// <summary>
        /// Endpoint: [POST] /api/transactions/sending/estimated
        /// </summary>
        public EstimateSendingTransactionResponse(IDictionary<AssetId, CoinsAmount> assetEstimatedFee)
        {
            AssetEstimatedFee = assetEstimatedFee ?? throw new ArgumentNullException(nameof(assetEstimatedFee));
        }
    }
}

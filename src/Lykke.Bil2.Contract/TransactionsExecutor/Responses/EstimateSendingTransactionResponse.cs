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
        /// Estimated transaction fee for the particular asset ID.
        /// </summary>
        [JsonProperty("estimatedFee")]
        public IReadOnlyDictionary<AssetId, CoinsAmount> EstimatedFee { get; }

        /// <summary>
        /// Endpoint: [POST] /api/transactions/sending/estimated
        /// </summary>
        public EstimateSendingTransactionResponse(IReadOnlyDictionary<AssetId, CoinsAmount> estimatedFee)
        {
            EstimatedFee = estimatedFee ?? throw new ArgumentNullException(nameof(estimatedFee));
        }
    }
}

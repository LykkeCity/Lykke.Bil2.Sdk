using System;
using System.Collections.Generic;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Endpoints:
    /// - [POST] /api/transactions/estimated/transfers/amount
    /// - [POST] /api/transactions/estimated/transfers/coins
    /// </summary>
    public class EstimateTransactionResponse
    {
        /// <summary>
        /// Estimated transaction fee for the particular asset ID.
        /// </summary>
        [JsonProperty("estimatedFee")]
        public IReadOnlyDictionary<AssetId, CoinsAmount> EstimatedFee { get; }

        /// <summary>
        /// Endpoints:
        /// - [POST] /api/transactions/estimated/transfers/amount
        /// - [POST] /api/transactions/estimated/transfers/coins
        /// </summary>
        public EstimateTransactionResponse(IReadOnlyDictionary<AssetId, CoinsAmount> estimatedFee)
        {
            EstimatedFee = estimatedFee ?? throw new ArgumentNullException(nameof(estimatedFee));
        }
    }
}

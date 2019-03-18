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
        /// Estimated transaction fees for the particular asset.
        /// </summary>
        [JsonProperty("estimatedFees")]
        public IReadOnlyCollection<Fee> EstimatedFees { get; }

        /// <summary>
        /// Endpoints:
        /// - [POST] /api/transactions/estimated/transfers/amount
        /// - [POST] /api/transactions/estimated/transfers/coins
        /// </summary>
        /// <param name="estimatedFees">Estimated transaction fee for the particular asset.</param>
        public EstimateTransactionResponse(IReadOnlyCollection<Fee> estimatedFees)
        {
            if(estimatedFees == null)
                throw new ArgumentNullException(nameof(estimatedFees));

            FeesValidator.ValidateFeesInResponse(estimatedFees);

            EstimatedFees = estimatedFees;
        }
    }
}

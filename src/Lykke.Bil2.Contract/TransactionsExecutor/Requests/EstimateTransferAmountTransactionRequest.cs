using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/estimated/transfers/amount
    /// </summary>
    public class EstimateTransferAmountTransactionRequest
    {
        /// <summary>
        /// Transaction transfers.
        /// </summary>
        [JsonProperty("transfers")]
        public IReadOnlyCollection<Transfer> Transfers { get; }

        /// <summary>
        /// Endpoint: [POST] /api/transactions/estimated/transfers/amount
        /// </summary>
        /// <param name="transfers">Transaction transfers.</param>
        public EstimateTransferAmountTransactionRequest(IReadOnlyCollection<Transfer> transfers)
        {
            TransactionTransfersValidator.Validate(transfers);

            Transfers = transfers;
        }
    }
}

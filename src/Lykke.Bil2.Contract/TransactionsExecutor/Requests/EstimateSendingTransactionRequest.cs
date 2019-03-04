using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/estimated/transfers
    /// </summary>
    public class EstimateSendingTransactionRequest
    {
        /// <summary>
        /// Transaction transfers.
        /// </summary>
        [JsonProperty("transfers")]
        public IReadOnlyCollection<Transfer> Transfers { get; }

        /// <summary>
        /// Endpoint: [POST] /api/transactions/estimated/transfers
        /// </summary>
        public EstimateSendingTransactionRequest(IReadOnlyCollection<Transfer> transfers)
        {
            TransactionTransfersValidator.Validate(transfers);

            Transfers = transfers;
        }
    }
}

using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Endpoint: [GET] /api/transactions/{transactionId}/state
    /// </summary>
    [PublicAPI]
    public class TransactionStateResponse
    {
        /// <summary>
        /// State of the transaction.
        /// </summary>
        [JsonProperty("state")]
        public TransactionState State { get; }

        /// <summary>
        /// Endpoint: [GET] /api/transactions/{transactionId}/state
        /// </summary>
        /// <param name="state">State of the transaction.</param>
        public TransactionStateResponse(TransactionState state)
        {
            State = state;
        }
    }
}

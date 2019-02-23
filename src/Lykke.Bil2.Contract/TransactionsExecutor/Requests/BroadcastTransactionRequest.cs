using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/broadcasted
    /// </summary>
    [PublicAPI]
    public class BroadcastTransactionRequest
    {
        // The signed transaction.
        [JsonProperty("signedTransaction")]
        public Base58String SignedTransaction { get; }

        /// <summary>
        /// Endpoint: [POST] /api/transactions/broadcasted
        /// </summary>
        public BroadcastTransactionRequest(Base58String signedTransaction)
        {
            if (string.IsNullOrWhiteSpace(signedTransaction?.ToString()))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(signedTransaction));

            SignedTransaction = signedTransaction;
        }
    }
}

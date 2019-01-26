using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
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

        public BroadcastTransactionRequest(Base58String signedTransaction)
        {
            if (string.IsNullOrWhiteSpace(signedTransaction))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(signedTransaction));

            SignedTransaction = signedTransaction;
        }
    }
}

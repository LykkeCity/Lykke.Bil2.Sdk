using System;
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
        public Base64String SignedTransaction { get; }

        public BroadcastTransactionRequest(Base64String signedTransaction)
        {
            if (string.IsNullOrWhiteSpace(signedTransaction))
                throw new ArgumentException("Should be not empty string", nameof(signedTransaction));

            SignedTransaction = signedTransaction;
        }
    }
}

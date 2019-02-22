using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/receiving/built
    /// </summary>
    [PublicAPI]
    public class BuildReceivingTransactionResponse
    {
        /// <summary>
        /// Implementation specific transaction context. 
        /// </summary>
        [JsonProperty("transactionContext")]
        public Base58String TransactionContext { get; }

        public BuildReceivingTransactionResponse(Base58String transactionContext)
        {
            if (string.IsNullOrWhiteSpace(transactionContext?.ToString()))
                throw new ArgumentException("Should be not empty string", nameof(transactionContext));

            TransactionContext = transactionContext;
        }
    }
}

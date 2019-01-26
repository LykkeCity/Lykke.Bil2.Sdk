using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses
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
            if (string.IsNullOrWhiteSpace(transactionContext))
                throw new ArgumentException("Should be not empty string", nameof(transactionContext));

            TransactionContext = transactionContext;
        }
    }
}

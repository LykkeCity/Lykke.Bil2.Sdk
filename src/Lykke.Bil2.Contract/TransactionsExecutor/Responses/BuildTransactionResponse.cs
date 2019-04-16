using System;
using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Endpoints:
    /// - [POST] /api/transactions/built/transfers/amount
    /// - [POST] /api/transactions/built/transfers/coins
    /// </summary>
    [PublicAPI]
    public class BuildTransactionResponse
    {
        /// <summary>
        /// Implementation specific transaction context. 
        /// </summary>
        [JsonProperty("transactionContext")]
        public Base58String TransactionContext { get; }

        /// <summary>
        /// Endpoints:
        /// - [POST] /api/transactions/built/transfers/amount
        /// - [POST] /api/transactions/built/transfers/coins
        /// </summary>
        /// <param name="transactionContext">Implementation specific transaction context.</param>
        public BuildTransactionResponse(Base58String transactionContext)
        {
            if (string.IsNullOrWhiteSpace(transactionContext?.ToString()))
                throw new ArgumentException("Should be not empty string", nameof(transactionContext));

            TransactionContext = transactionContext;
        }
    }
}

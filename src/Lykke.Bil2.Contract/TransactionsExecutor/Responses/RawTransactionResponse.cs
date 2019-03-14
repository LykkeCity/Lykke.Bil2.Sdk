using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Endpoint: [GET] /api/transactions/{transactionId}/raw
    /// </summary>
    [PublicAPI]
    public class RawTransactionResponse
    {
        /// <summary>
        /// Raw transaction.
        /// </summary>
        [JsonProperty("raw")]
        public Base58String Raw { get; }

        /// <summary>
        /// Endpoint: [GET] /api/transactions/{transactionId}/raw
        /// </summary>
        /// <param name="raw">Raw transaction.</param>
        public RawTransactionResponse(Base58String raw)
        {
            if (string.IsNullOrWhiteSpace(raw?.ToString()))
                throw new ArgumentException("Should be not empty string", nameof(raw));

            Raw = raw;
        }
    }
}

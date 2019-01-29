using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Endpoint: [GET] /api/transactions/{transactionHash}/raw
    /// </summary>
    [PublicAPI]
    public class RawTransactionResponse
    {
        /// <summary>
        /// Raw transaction.
        /// </summary>
        [JsonProperty("raw")]
        public Base58String Raw { get; }

        public RawTransactionResponse(Base58String raw)
        {
            if (string.IsNullOrWhiteSpace(raw?.ToString()))
                throw new ArgumentException("Should be not empty string", nameof(raw));

            Raw = raw;
        }
    }
}

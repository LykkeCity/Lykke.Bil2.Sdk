using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.SignService.Requests
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/signed
    /// </summary>
    [PublicAPI]
    public class SignTransactionRequest
    {
        /// <summary>
        /// Private keys of the addresses which should sign the transaction.
        /// Multiple keys can be used for the/ transactions with multiple inputs.
        /// </summary>
        [JsonProperty("privateKeys")]
        public ICollection<string> PrivateKeys { get; }

        /// <summary>
        /// Implementation specific transaction context.
        /// </summary>
        [JsonProperty("transactionContext")]
        public Base64String TransactionContext { get; }

        public SignTransactionRequest(ICollection<string> privateKeys, Base64String transactionContext)
        {
            if (privateKeys == null || !privateKeys.Any() || privateKeys.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Should be not empty collection of not empty strings", nameof(privateKeys));

            if (string.IsNullOrWhiteSpace(transactionContext))
                throw new ArgumentException("Should be not empty string", nameof(transactionContext));

            PrivateKeys = privateKeys;
            TransactionContext = transactionContext;
        }
    }
}

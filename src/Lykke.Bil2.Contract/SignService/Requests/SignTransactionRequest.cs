using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.SignService.Requests
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
        public IReadOnlyCollection<EncryptedString> PrivateKeys { get; }

        /// <summary>
        /// Implementation specific transaction context.
        /// </summary>
        [JsonProperty("transactionContext")]
        public Base58String TransactionContext { get; }

        /// <summary>
        /// Endpoint: [POST] /api/transactions/signed
        /// </summary>
        /// <param name="privateKeys">
        /// Private keys of the addresses which should sign the transaction.
        /// Multiple keys can be used for the/ transactions with multiple inputs.
        /// </param>
        /// <param name="transactionContext">
        /// Implementation specific transaction context.
        /// </param>
        public SignTransactionRequest(IReadOnlyCollection<EncryptedString> privateKeys, Base58String transactionContext)
        {
            if (privateKeys == null || !privateKeys.Any() || privateKeys.Any(x => x == null))
                throw RequestValidationException.ShouldBeNotEmptyCollection(nameof(privateKeys));

            if (string.IsNullOrWhiteSpace(transactionContext?.ToString()))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(transactionContext));

            PrivateKeys = privateKeys;
            TransactionContext = transactionContext;
        }
    }
}

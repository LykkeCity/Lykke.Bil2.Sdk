using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.SignService.Responses
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/signed
    /// </summary>
    [PublicAPI]
    public class SignTransactionResponse
    {
        /// <summary>
        /// Implementation specific signed transaction.
        /// </summary>
        [JsonProperty("signedTransaction")]
        public Base58String SignedTransaction { get; }

        /// <summary>
        /// Hash of the signed transaction in the blockchain.
        /// </summary>
        [JsonProperty("transactionHash")]
        public string TransactionHash { get; }

        /// <summary>
        /// Endpoint: [POST] /api/transactions/signed
        /// </summary>
        public SignTransactionResponse(Base58String signedTransaction, string transactionHash)
        {
            if (string.IsNullOrWhiteSpace(signedTransaction?.ToString()))
                throw new ArgumentException("Should be not empty string", nameof(signedTransaction));

            if (string.IsNullOrWhiteSpace(transactionHash))
                throw new ArgumentException("Should be not empty string", nameof(transactionHash));

            SignedTransaction = signedTransaction;
            TransactionHash = transactionHash;
        }
    }
}

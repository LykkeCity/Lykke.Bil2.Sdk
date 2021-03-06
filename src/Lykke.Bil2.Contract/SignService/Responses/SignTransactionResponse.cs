﻿using System;
using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;
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
        public Base64String SignedTransaction { get; }

        /// <summary>
        /// ID of the signed transaction in the blockchain.
        /// </summary>
        [JsonProperty("transactionId")]
        public TransactionId TransactionId { get; }

        /// <summary>
        /// Endpoint: [POST] /api/transactions/signed
        /// </summary>
        /// <param name="signedTransaction">Implementation specific signed transaction.</param>
        /// <param name="transactionId">ID of the signed transaction in the blockchain.</param>
        public SignTransactionResponse(Base64String signedTransaction, TransactionId transactionId)
        {
            if (string.IsNullOrWhiteSpace(signedTransaction?.ToString()))
                throw new ArgumentException("Should be not empty string", nameof(signedTransaction));

            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Should be not empty string", nameof(transactionId));

            SignedTransaction = signedTransaction;
            TransactionId = transactionId;
        }
    }
}

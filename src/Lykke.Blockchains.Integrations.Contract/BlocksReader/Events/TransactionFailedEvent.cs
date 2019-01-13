using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.BlocksReader.Events
{
    /// <summary>
    /// Should be published for each failed transaction in the block being read.
    /// </summary>
    [PublicAPI]
    public class TransactionFailedEvent
    {
        /// <summary>
        /// Hash of the block.
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; }

        /// <summary>
        /// One-based number of the transaction in the block.
        /// </summary>
        [JsonProperty("transactionNumber")]
        public int TransactionNumber { get; }

        /// <summary>
        /// Hash of the transaction.
        /// </summary>
        [JsonProperty("transactionHash")]
        public string TransactionHash { get; }

        /// <summary>
        /// Code of the error.
        /// </summary>
        [JsonProperty("errorCode")]
        public TransactionBroadcastingError ErrorCode { get; }

        /// <summary>
        /// Clean error description.
        /// </summary>
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; }

        public TransactionFailedEvent(
            string blockHash,
            int transactionNumber,
            string transactionHash,
            TransactionBroadcastingError errorCode,
            string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(blockHash))
                throw new ArgumentException("Should be not empty string", nameof(blockHash));

            if (transactionNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(transactionNumber), transactionNumber, "Should be positive number");

            if (string.IsNullOrWhiteSpace(transactionHash))
                throw new ArgumentException("Should be not empty string", nameof(transactionHash));

            BlockHash = blockHash;
            TransactionNumber = transactionNumber;
            TransactionHash = transactionHash;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}

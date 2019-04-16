using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// Should be published for each failed transaction in the block being read.
    /// </summary>
    [PublicAPI]
    public class TransactionFailedEvent
    {
        /// <summary>
        /// ID of the block.
        /// </summary>
        [JsonProperty("blockId")]
        public BlockId BlockId { get; }

        /// <summary>
        /// One-based number of the transaction in the block.
        /// </summary>
        [JsonProperty("transactionNumber")]
        public int TransactionNumber { get; }

        /// <summary>
        /// ID of the transaction.
        /// </summary>
        [JsonProperty("transactionId")]
        public TransactionId TransactionId { get; }

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

        /// <summary>
        /// Optional.
        /// Fees in the particular asset, that was spent for the transaction.
        /// Can be omitted, if there was no fee spent for the transaction.
        /// </summary>
        [CanBeNull]
        [JsonProperty("fees")]
        public IReadOnlyCollection<Fee> Fees { get; }

        /// <summary>
        /// Should be published for each failed transaction in the block being read.
        /// </summary>
        /// <param name="blockId">ID of the block.</param>
        /// <param name="transactionNumber">One-based number of the transaction in the block.</param>
        /// <param name="transactionId">ID of the transaction.</param>
        /// <param name="errorCode">Code of the error.</param>
        /// <param name="errorMessage">
        /// Optional.
        /// Fee in the particular asset ID, that was spent for the transaction.
        /// Can be omitted, if there was no fee spent for the transaction.
        /// </param>
        /// <param name="fees">
        /// Optional.
        /// Fees in the particular asset, that was spent for the transaction.
        /// Can be omitted, if there was no fee spent for the transaction.
        /// </param>
        public TransactionFailedEvent(
            BlockId blockId,
            int transactionNumber,
            TransactionId transactionId,
            TransactionBroadcastingError errorCode,
            string errorMessage,
            IReadOnlyCollection<Fee> fees = null)
        {
            if (transactionNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(transactionNumber), transactionNumber, "Should be zero or positive number");

            FeesValidator.ValidateFeesInResponse(fees);

            BlockId = blockId ?? throw new ArgumentNullException(nameof(blockId));
            TransactionNumber = transactionNumber;
            TransactionId = transactionId ?? throw new ArgumentNullException(nameof(transactionId));
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Fees = fees;
        }
    }
}

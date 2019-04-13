using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// "Transfer amount" transactions model.
    /// Should be published for each executed transaction in the block being read if
    /// integration uses “transfer amount” transactions model. Integration should either
    /// support “transfer coins” or “transfer amount” transactions model.
    /// </summary>
    [PublicAPI]
    public class TransferAmountTransactionExecutedEvent
    {
        /// <summary>
        /// ID of the block.
        /// </summary>
        [JsonProperty("blockId")]
        public string BlockId { get; }

        /// <summary>
        /// Number of the transaction in the block.
        /// </summary>
        [JsonProperty("transactionNumber")]
        public int TransactionNumber { get; }

        /// <summary>
        /// ID of the transaction.
        /// </summary>
        [JsonProperty("transactionId")]
        public string TransactionId { get; }

        /// <summary>
        /// Balance changing operations.
        /// </summary>
        [JsonProperty("balanceChanges")]
        public IReadOnlyCollection<BalanceChange> BalanceChanges { get; }

        /// <summary>
        /// Fees in the particular asset, that was spent for the transaction.
        /// </summary>
        [JsonProperty("fees")]
        public IReadOnlyCollection<Fee> Fees { get; }

        /// <summary>
        /// Optional.
        /// Flag which indicates, if transaction is irreversible.
        /// </summary>
        [CanBeNull]
        [JsonProperty("isIrreversible")]
        public bool? IsIrreversible { get; }

        /// <summary>
        /// "Transfer amount" transactions model.
        /// Should be published for each executed transaction in the block being read if
        /// integration uses “transfer amount” transactions model. Integration should either
        /// support “transfer coins” or “transfer amount” transactions model.
        /// </summary>
        /// <param name="blockId">ID of the block.</param>
        /// <param name="transactionNumber">Number of the transaction in the block.</param>
        /// <param name="transactionId">ID of the transaction.</param>
        /// <param name="balanceChanges">Balance changing operations.</param>
        /// <param name="fees">Fees in the particular asset, that was spent for the transaction.</param>
        /// <param name="isIrreversible">
        /// Optional.
        /// Flag which indicates, if transaction is irreversible.
        /// </param>
        public TransferAmountTransactionExecutedEvent(
            string blockId,
            int transactionNumber,
            string transactionId,
            IReadOnlyCollection<BalanceChange> balanceChanges,
            IReadOnlyCollection<Fee> fees,
            bool? isIrreversible = null)
        {
            if (string.IsNullOrWhiteSpace(blockId))
                throw new ArgumentException("Should be not empty string", nameof(blockId));

            if (transactionNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(transactionNumber), transactionNumber, "Should be zero or positive number");

            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Should be not empty string", nameof(transactionId));

            if (fees == null)
                throw new ArgumentNullException(nameof(fees));
            
            BalanceChangesValidator.Validate(balanceChanges);
            FeesValidator.ValidateFeesInResponse(fees);
            
            BlockId = blockId;
            TransactionNumber = transactionNumber;
            TransactionId = transactionId;
            BalanceChanges = balanceChanges;
            Fees = fees;
            IsIrreversible = isIrreversible;
        }
    }
}

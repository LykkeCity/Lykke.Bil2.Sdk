using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// Should be published for each executed transaction in the block being read.
    /// </summary>
    [PublicAPI]
    public class TransactionExecutedEvent
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
        /// Balance changing operations.
        /// </summary>
        [JsonProperty("balanceChanges")]
        public IReadOnlyCollection<BalanceChange> BalanceChanges { get; }

        /// <summary>
        /// Optional.
        /// ID of the balance changes which was cancelled.
        /// </summary>
        [CanBeNull]
        [JsonProperty("cancelledBalanceChanges")]
        public IReadOnlyCollection<BalanceChangeId> CancelledBalanceChanges { get; }

        /// <summary>
        /// Optional.
        /// Fee in the particular asset ID, that was spent for the transaction.
        /// Can be omitted, if fee can be determined from the balance changes and cancellations.
        /// </summary>
        [CanBeNull]
        [JsonProperty("fee")]
        public IReadOnlyDictionary<AssetId, CoinsAmount> Fee { get; }

        /// <summary>
        /// Optional.
        /// Flag which indicates, if transaction is irreversible.
        /// </summary>
        [CanBeNull]
        [JsonProperty("isIrreversible")]
        public bool? IsIrreversible { get; }

        /// <summary>
        /// Optional.
        /// Type of the transaction.
        /// </summary>
        [CanBeNull]
        [JsonProperty("transactionType")]
        public TransactionType? TransactionType { get; }

        /// <summary>
        /// Should be published for each executed transaction in the block being read.
        /// </summary>
        public TransactionExecutedEvent(
            string blockHash,
            int transactionNumber,
            string transactionHash,
            IReadOnlyCollection<BalanceChange> balanceChanges,
            IReadOnlyCollection<BalanceChangeId> cancelledBalanceChanges = null,
            IReadOnlyDictionary<AssetId, CoinsAmount> fee = null,
            bool? isIrreversible = null,
            TransactionType? transactionType = null)
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
            BalanceChanges = balanceChanges ?? throw new ArgumentNullException(nameof(balanceChanges));
            CancelledBalanceChanges = cancelledBalanceChanges;
            Fee = fee;
            IsIrreversible = isIrreversible;
            TransactionType = transactionType;
        }
    }
}

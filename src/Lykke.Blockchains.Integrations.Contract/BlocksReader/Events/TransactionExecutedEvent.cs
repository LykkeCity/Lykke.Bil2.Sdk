using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.BlocksReader.Events
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
        public ICollection<BalanceChange> BalanceChanges { get; }

        /// <summary>
        /// Optional.
        /// ID of the balance changes which was cancelled.
        /// </summary>
        [CanBeNull]
        [JsonProperty("cancelledBalanceChanges")]
        public ICollection<string> CancelledBalanceChanges { get; }

        /// <summary>
        /// Optional.
        /// Fee in the particular asset ID, that was spent for the transaction.
        /// Can be omitted, if fee can be determined from the balance changes and cancellations.
        /// </summary>
        [CanBeNull]
        [JsonProperty("fee")]
        public IDictionary<string, CoinsAmount> Fee { get; }

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

        public TransactionExecutedEvent(
            string blockHash,
            int transactionNumber,
            string transactionHash,
            ICollection<BalanceChange> balanceChanges,
            ICollection<string> cancelledBalanceChanges = null,
            IDictionary<string, CoinsAmount> fee = null,
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

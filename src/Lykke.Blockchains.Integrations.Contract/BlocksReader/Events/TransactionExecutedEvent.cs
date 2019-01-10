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
        public string BlockHash { get; set; }

        /// <summary>
        /// One-based number of the transaction in the block.
        /// </summary>
        [JsonProperty("transactionNumber")]
        public int TransactionNumber { get; set; }

        /// <summary>
        /// Hash of the transaction.
        /// </summary>
        [JsonProperty("transactionHash")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// Balance changing operations.
        /// </summary>
        [JsonProperty("balanceChanges")]
        public ICollection<BalanceChange> BalanceChanges { get; set; }

        /// <summary>
        /// Optional.
        /// ID of the balance changes which was cancelled.
        /// </summary>
        [CanBeNull]
        [JsonProperty("cancelledBalanceChanges")]
        public ICollection<string> CancelledBalanceChanges { get; set; }

        /// <summary>
        /// Optional.
        /// Fee in the particular asset ID, that was spent for the transaction.
        /// Can be omitted, if fee can be determined from the balance changes and cancellations.
        /// </summary>
        [CanBeNull]
        [JsonProperty("fee")]
        public IDictionary<string, CoinsAmount> Fee { get; set; }

        /// <summary>
        /// Optional.
        /// Flag which indicates, if transaction is irreversible.
        /// </summary>
        [CanBeNull]
        [JsonProperty("isIrreversible")]
        public bool? IsIrreversible { get; set; }

        /// <summary>
        /// Optional.
        /// Type of the transaction.
        /// </summary>
        [CanBeNull]
        [JsonProperty("transactionType")]
        public TransactionType? TransactionType { get; set; }
    }
}

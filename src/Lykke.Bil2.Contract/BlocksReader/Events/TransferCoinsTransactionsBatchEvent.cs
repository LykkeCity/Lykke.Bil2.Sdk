using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// "Transfer coins" transactions model.
    /// Batch of the transactions read from the block.
    /// </summary>
    [PublicAPI, DataContract]
    public class TransferCoinsTransactionsBatchEvent
    {
        /// <summary>
        /// ID of the block.
        /// </summary>
        [DataMember(Order = 0)]
        public BlockId BlockId { get; }

        /// <summary>
        /// Executed transactions.
        /// </summary>
        [DataMember(Order = 1)]
        public IReadOnlyCollection<TransferCoinsExecutedTransaction> TransferCoinsExecutedTransactions { get; }

        /// <summary>
        /// Failed transactions.
        /// </summary>
        [DataMember(Order = 2)]
        public IReadOnlyCollection<FailedTransaction> FailedTransactions { get; }

        /// <summary>
        /// "Transfer coins" transactions model.
        /// Batch of the transactions read from the block.
        /// </summary>
        /// <param name="blockId">ID of the block.</param>
        /// <param name="transferCoinsExecutedTransactions">Executed transactions.</param>
        /// <param name="failedTransactions">Failed transactions.</param>
        public TransferCoinsTransactionsBatchEvent(
            BlockId blockId,
            IReadOnlyCollection<TransferCoinsExecutedTransaction> transferCoinsExecutedTransactions,
            IReadOnlyCollection<FailedTransaction> failedTransactions)
        {
            BlockId = blockId ?? throw new ArgumentNullException(nameof(blockId));
            TransferCoinsExecutedTransactions = transferCoinsExecutedTransactions ?? throw new ArgumentNullException(nameof(transferCoinsExecutedTransactions));
            FailedTransactions = failedTransactions ?? throw new ArgumentNullException(nameof(failedTransactions));
        }

    }
}
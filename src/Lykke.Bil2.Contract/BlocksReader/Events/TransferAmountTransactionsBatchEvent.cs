using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// "Transfer amount" transactions model.
    /// Batch of the transactions read from the block.
    /// </summary>
    [PublicAPI, DataContract]
    public class TransferAmountTransactionsBatchEvent
    {
        /// <summary>
        /// ID of the block.
        /// </summary>
        [DataMember(Order = 0)]
        public BlockId BlockId { get; }

        /// <summary>
        /// Number of the bock.
        /// </summary>
        [DataMember(Order = 1)]
        public long BlockNumber { get; }

        /// <summary>
        /// Executed transactions.
        /// </summary>
        [DataMember(Order = 2)]
        public IReadOnlyCollection<TransferAmountExecutedTransaction> TransferAmountExecutedTransactions { get; }

        /// <summary>
        /// Failed transactions.
        /// </summary>
        [DataMember(Order = 3)]
        public IReadOnlyCollection<FailedTransaction> FailedTransactions { get; }

        /// <summary>
        /// "Transfer coins" transactions model.
        /// Batch of the transactions read from the block.
        /// </summary>
        /// <param name="blockId">ID of the block.</param>
        /// <param name="blockNumber">Number of the block.</param>
        /// <param name="transferAmountExecutedTransactions">Executed transactions.</param>
        /// <param name="failedTransactions">Failed transactions.</param>
        public TransferAmountTransactionsBatchEvent(
            BlockId blockId,
            long blockNumber,
            IReadOnlyCollection<TransferAmountExecutedTransaction> transferAmountExecutedTransactions,
            IReadOnlyCollection<FailedTransaction> failedTransactions)
        {
            if (BlockNumber < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockNumber), blockNumber, "Should be zero or positive number");
            }

            BlockId = blockId ?? throw new ArgumentNullException(nameof(blockId));
            BlockNumber = blockNumber;
            TransferAmountExecutedTransactions = transferAmountExecutedTransactions ?? throw new ArgumentNullException(nameof(transferAmountExecutedTransactions));
            FailedTransactions = failedTransactions ?? throw new ArgumentNullException(nameof(failedTransactions));
        }
    }
}

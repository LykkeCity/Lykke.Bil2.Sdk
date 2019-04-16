using System;
using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// Should be published when a block header has been read.
    /// </summary>
    [PublicAPI]
    public class BlockHeaderReadEvent
    {
        /// <summary>
        /// Number of the block.
        /// </summary>
        [JsonProperty("blockNumber")]
        public long BlockNumber { get; }

        /// <summary>
        /// ID of the block.
        /// </summary>
        [JsonProperty("blockId")]
        public BlockId BlockId { get; }

        /// <summary>
        /// Moment when the block is mined.
        /// </summary>
        [JsonProperty("blockMiningMoment")]
        public DateTime BlockMiningMoment { get; }

        /// <summary>
        /// Size of the block in bytes.
        /// </summary>
        [JsonProperty("blockSize")]
        public int BlockSize { get; }

        /// <summary>
        /// Count of the transactions in the block.
        /// </summary>
        [JsonProperty("blockTransactionsCount")]
        public int BlockTransactionsCount { get; }

        /// <summary>
        /// Optional.
        /// ID of the previous block. Optional only for the first block (blockNumber 0 or 1)
        /// </summary>
        [CanBeNull]
        [JsonProperty("previousBlockId")]
        public BlockId PreviousBlockId { get; }

        /// <summary>
        /// Should be published when a block header has been read.
        /// </summary>
        /// <param name="blockNumber">Number of the block.</param>
        /// <param name="blockId">ID of the block.</param>
        /// <param name="blockMiningMoment">Moment when the block is mined.</param>
        /// <param name="blockSize">Size of the block in bytes.</param>
        /// <param name="blockTransactionsCount">Count of the transactions in the block.</param>
        /// <param name="previousBlockId">
        /// Optional.
        /// ID of the previous block. Optional only for the first block (blockNumber 0 or 1)
        /// </param>
        public BlockHeaderReadEvent(
            long blockNumber,
            BlockId blockId,
            DateTime blockMiningMoment,
            int blockSize,
            int blockTransactionsCount,
            BlockId previousBlockId = null)
        {
            if (blockNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(blockNumber), blockNumber, "Should be zero or positive number");

            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(blockSize), blockSize, "Should be positive number");

            if (blockTransactionsCount < 0)
                throw new ArgumentOutOfRangeException(nameof(blockTransactionsCount), blockTransactionsCount, "Should be positive number or zero");

            if (blockNumber > 1 && previousBlockId == null)
                throw new ArgumentNullException(nameof(previousBlockId), "Should be not null for the not first block");

            BlockNumber = blockNumber;
            BlockId = blockId ?? throw new ArgumentNullException(nameof(blockId));
            BlockMiningMoment = blockMiningMoment;
            BlockSize = blockSize;
            BlockTransactionsCount = blockTransactionsCount;
            PreviousBlockId = previousBlockId;
        }
    }
}

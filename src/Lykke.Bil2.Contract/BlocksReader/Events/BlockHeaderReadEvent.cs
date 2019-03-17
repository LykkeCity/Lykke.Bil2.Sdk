using System;
using JetBrains.Annotations;
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
        public string BlockId { get; }

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
        public string PreviousBlockId { get; }

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
            string blockId,
            DateTime blockMiningMoment,
            int blockSize,
            int blockTransactionsCount,
            string previousBlockId = null)
        {
            if (blockNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(blockNumber), blockNumber, "Should be zero or positive number");

            if (string.IsNullOrWhiteSpace(blockId))
                throw new ArgumentException("Should be not empty string", nameof(blockId));

            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(blockSize), blockSize, "Should be positive number");

            if (blockTransactionsCount < 0)
                throw new ArgumentOutOfRangeException(nameof(blockTransactionsCount), blockTransactionsCount, "Should be positive number or zero");

            if (blockNumber > 1 && string.IsNullOrWhiteSpace(previousBlockId))
                throw new ArgumentException("Should be not empty string for the not first block", nameof(previousBlockId));

            BlockNumber = blockNumber;
            BlockId = blockId;
            BlockMiningMoment = blockMiningMoment;
            BlockSize = blockSize;
            BlockTransactionsCount = blockTransactionsCount;
            PreviousBlockId = previousBlockId;
        }
    }
}

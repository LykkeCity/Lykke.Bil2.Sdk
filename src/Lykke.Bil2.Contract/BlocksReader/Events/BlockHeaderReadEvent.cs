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
        /// Number of the transactions in the block.
        /// </summary>
        [JsonProperty("blockTransactionsNumber")]
        public int BlockTransactionsNumber { get; }

        /// <summary>
        /// Optional.
        /// ID of the previous block. Optional only for the first block (blockNumber 0 or 1)
        /// </summary>
        [CanBeNull]
        [JsonProperty("previousBlockId")]
        public string PreviousBlockId { get; }

        public BlockHeaderReadEvent(
            long blockNumber,
            string blockId,
            DateTime blockMiningMoment,
            int blockSize,
            int blockTransactionsNumber,
            string previousBlockId = null)
        {
            if (blockNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(blockNumber), blockNumber, "Should be zero or positive number");

            if (string.IsNullOrWhiteSpace(blockId))
                throw new ArgumentException("Should be not empty string", nameof(blockId));

            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(blockSize), blockSize, "Should be positive number");

            if (blockTransactionsNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(blockTransactionsNumber), blockTransactionsNumber, "Should be positive number");

            if (blockNumber > 1 && string.IsNullOrWhiteSpace(previousBlockId))
                throw new ArgumentException("Should be not empty string for the not first block", nameof(previousBlockId));

            BlockNumber = blockNumber;
            BlockId = blockId;
            BlockMiningMoment = blockMiningMoment;
            BlockSize = blockSize;
            BlockTransactionsNumber = blockTransactionsNumber;
            PreviousBlockId = previousBlockId;
        }
    }
}

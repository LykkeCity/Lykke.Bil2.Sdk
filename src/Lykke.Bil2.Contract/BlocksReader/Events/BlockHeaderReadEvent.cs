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
        /// Hash of the block.
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; }

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
        /// Hash of the previous block. Optional only for the first block (blokNumber 0 or 1)
        /// </summary>
        [CanBeNull]
        [JsonProperty("previousBlockHash")]
        public string PreviousBlockHash { get; }

        public BlockHeaderReadEvent(
            long blockNumber,
            string blockHash,
            DateTime blockMiningMoment,
            int blockSize,
            int blockTransactionsNumber,
            string previousBlockHash = null)
        {
            if (blockNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(blockNumber), blockNumber, "Should be zero or positive number");

            if (string.IsNullOrWhiteSpace(blockHash))
                throw new ArgumentException("Should be not empty string", nameof(blockHash));

            if (blockSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(blockSize), blockSize, "Should be positive number");

            if (blockTransactionsNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(blockTransactionsNumber), blockTransactionsNumber, "Should be positive number");

            if (blockNumber > 1 && string.IsNullOrWhiteSpace(previousBlockHash))
                throw new ArgumentException("Should be not empty string for the not first block", nameof(previousBlockHash));

            BlockNumber = blockNumber;
            BlockHash = blockHash;
            BlockMiningMoment = blockMiningMoment;
            BlockSize = blockSize;
            BlockTransactionsNumber = blockTransactionsNumber;
            PreviousBlockHash = previousBlockHash;
        }
    }
}

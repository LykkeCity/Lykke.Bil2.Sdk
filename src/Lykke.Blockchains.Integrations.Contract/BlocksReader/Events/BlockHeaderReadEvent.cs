using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.BlocksReader.Events
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
        public long BlockNumber { get; set; }

        /// <summary>
        /// Hash of the block.
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        /// <summary>
        /// Hash of the previous block.
        /// </summary>
        [JsonProperty("previousBlockHash")]
        public string PreviousBlockHash { get; set; }

        /// <summary>
        /// Moment when the block is mined.
        /// </summary>
        [JsonProperty("blockMiningMoment")]
        public DateTime BlockMiningMoment { get; set; }

        /// <summary>
        /// Size of the block in bytes.
        /// </summary>
        [JsonProperty("blockSize")]
        public int BlockSize { get; set; }

        /// <summary>
        /// Number of the transactions in the block.
        /// </summary>
        [JsonProperty("blockTransactionsNumber")]
        public int BlockTransactionsNumber { get; set; }
    }
}

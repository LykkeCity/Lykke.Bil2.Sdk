using System;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// Should be published when last irreversible block number is updated.
    /// </summary>
    public class LastIrreversibleBlockUpdatedEvent
    {
        /// <summary>
        /// Number of the last irreversible block.
        /// </summary>
        [JsonProperty("blockNumber")]
        public long BlockNumber { get; }

        /// <summary>
        /// ID of the last irreversible block.
        /// </summary>
        [JsonProperty("blockId")]
        public string BlockId { get; }

        /// <summary>
        /// Should be published when last irreversible block number is updated.
        /// </summary>
        /// <param name="blockNumber">Number of the last irreversible block.</param>
        /// <param name="blockId">ID of the last irreversible block.</param>
        public LastIrreversibleBlockUpdatedEvent(long blockNumber, string blockId)
        {
            if (blockNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(blockNumber), blockNumber, "Should be zero or positive number");

            if (string.IsNullOrWhiteSpace(blockId))
                throw new ArgumentException("Should be not empty string", nameof(blockId));

            BlockNumber = blockNumber;
            BlockId = blockId;
        }
    }
}

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
        /// Hash of the last irreversible block.
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; }

        public LastIrreversibleBlockUpdatedEvent(long blockNumber, string blockHash)
        {
            if (blockNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(blockNumber), blockNumber, "Should be zero or positive number");

            if (string.IsNullOrWhiteSpace(blockHash))
                throw new ArgumentException("Should be not empty string", nameof(blockHash));

            BlockNumber = blockNumber;
            BlockHash = blockHash;
        }
    }
}

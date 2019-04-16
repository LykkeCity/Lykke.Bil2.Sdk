using System;
using Lykke.Bil2.SharedDomain;
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
        public BlockId BlockId { get; }

        /// <summary>
        /// Should be published when last irreversible block number is updated.
        /// </summary>
        /// <param name="blockNumber">Number of the last irreversible block.</param>
        /// <param name="blockId">ID of the last irreversible block.</param>
        public LastIrreversibleBlockUpdatedEvent(long blockNumber, BlockId blockId)
        {
            if (blockNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(blockNumber), blockNumber, "Should be zero or positive number");

            BlockNumber = blockNumber;
            BlockId = blockId ?? throw new ArgumentNullException(nameof(blockId));
        }
    }
}

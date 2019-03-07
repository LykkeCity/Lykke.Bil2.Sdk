using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// Should be published if block requested by ReadBlockCommand is not found.
    /// </summary>
    [PublicAPI]
    public class BlockNotFoundEvent
    {
        /// <summary>
        /// Number of the block.
        /// </summary>
        [JsonProperty("blockNumber")]
        public long BlockNumber { get; }

        /// <summary>
        /// Number of the block.
        /// </summary>
        public BlockNotFoundEvent(long blockNumber)
        {
            if (blockNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(blockNumber), blockNumber, "Should be zero or positive number");

            BlockNumber = blockNumber;
        }
    }
}
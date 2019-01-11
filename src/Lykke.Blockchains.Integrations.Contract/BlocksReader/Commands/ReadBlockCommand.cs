using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.BlocksReader.Commands
{
    /// <summary>
    /// Service should start reading of the specified block and send events corresponding to all transactions included in the block.
    /// </summary>
    [PublicAPI]
    public class ReadBlockCommand
    {
        /// <summary>
        /// Number of the block to read.
        /// </summary>
        [JsonProperty("blockNumber")]
        public long BlockNumber { get; }

        public ReadBlockCommand(long blockNumber)
        {
            if (blockNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(blockNumber), blockNumber, "Should be zero or positive number");

            BlockNumber = blockNumber;
        }
    }
}

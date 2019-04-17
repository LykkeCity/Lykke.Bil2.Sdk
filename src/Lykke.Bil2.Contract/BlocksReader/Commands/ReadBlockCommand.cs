using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Lykke.Bil2.Contract.BlocksReader.Commands
{
    /// <summary>
    /// Service should start reading of the specified block and send events corresponding to all transactions included in the block.
    /// </summary>
    [PublicAPI, DataContract]
    public class ReadBlockCommand
    {
        /// <summary>
        /// Number of the block to read.
        /// </summary>
        [DataMember(Order = 0)]
        public long BlockNumber { get; }

        /// <summary>
        /// Service should start reading of the specified block and send events corresponding to all transactions included in the block.
        /// </summary>
        /// <param name="blockNumber">
        /// Number of the block to read.
        /// </param>
        public ReadBlockCommand(long blockNumber)
        {
            if (blockNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(blockNumber), blockNumber, "Should be zero or positive number");

            BlockNumber = blockNumber;
        }
    }
}

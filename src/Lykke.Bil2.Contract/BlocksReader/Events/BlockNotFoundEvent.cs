using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// Should be published if block requested by ReadBlockCommand is not found.
    /// </summary>
    [PublicAPI, DataContract]
    public class BlockNotFoundEvent
    {
        /// <summary>
        /// Number of the block.
        /// </summary>
        [DataMember(Order = 0)]
        public long BlockNumber { get; }

        /// <summary>
        /// Should be published if block requested by ReadBlockCommand is not found.
        /// </summary>
        /// <param name="blockNumber">Number of the block.</param>
        public BlockNotFoundEvent(long blockNumber)
        {
            if (blockNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(blockNumber), blockNumber, "Should be zero or positive number");

            BlockNumber = blockNumber;
        }
    }
}

using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// Should be published when last irreversible block number is updated.
    /// </summary>
    [PublicAPI, DataContract]
    public class LastIrreversibleBlockUpdatedEvent
    {
        /// <summary>
        /// Number of the last irreversible block.
        /// </summary>
        [DataMember(Order = 0)]
        public long BlockNumber { get; }

        /// <summary>
        /// ID of the last irreversible block.
        /// </summary>
        [DataMember(Order = 1)]
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

using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.BlocksReader.Events
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
        public long BlockNumber { get; set; }

        /// <summary>
        /// Hash of the last irreversible block.
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }
    }
}
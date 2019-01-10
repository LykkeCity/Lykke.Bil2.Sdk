using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Transaction output.
    /// </summary>
    [PublicAPI]
    public class Output
    {
        /// <summary>
        /// Asset ID to transfer.
        /// </summary>
        [JsonProperty("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// Address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Optional.
        /// Address tag.
        /// </summary>
        [CanBeNull]
        [JsonProperty("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Optional.
        /// Type of the address tag.
        /// </summary>
        [CanBeNull]
        [JsonProperty("tagType")]
        public AddressTagType? TagType { get; set; }

        /// <summary>
        /// Amount to transfer to the given address.
        /// </summary>
        [JsonProperty("amount")]
        public CoinsAmount Amount { get; set; }
    }
}

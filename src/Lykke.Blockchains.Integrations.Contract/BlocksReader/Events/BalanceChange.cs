using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.BlocksReader.Events
{
    /// <summary>
    /// Change of the address balance made by a transaction
    /// </summary>
    [PublicAPI]
    public class BalanceChange
    {
        /// <summary>
        /// Unique across entire blockchain balance changing operation ID.
        /// For example, for Bitcoin it would be transactionHash + outputNumber.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// ID of the transfer within the transaction.
        /// Can group several balance changing operations into the single transfer,
        /// or can be just the output number.
        /// </summary>
        [JsonProperty("transferId")]
        public string TransferId { get; set; }

        /// <summary>
        /// ID of the asset.
        /// </summary>
        [JsonProperty("assetId")]
        public string AssetId { get; set; }

        /// <summary>
        /// Value for which the balance of the address was changed.
        /// Can be positive to increase the balance or negative to decrease the balance.
        /// </summary>
        [JsonProperty("value")]
        public CoinsChange Value { get; set; }

        /// <summary>
        /// Address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Optional.
        /// Tag of the address.
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
    }
}

using System;
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
        public string Id { get; }

        /// <summary>
        /// ID of the transfer within the transaction.
        /// Can group several balance changing operations into the single transfer,
        /// or can be just the output number.
        /// </summary>
        [JsonProperty("transferId")]
        public string TransferId { get; }

        /// <summary>
        /// ID of the asset.
        /// </summary>
        [JsonProperty("assetId")]
        public string AssetId { get; }

        /// <summary>
        /// Value for which the balance of the address was changed.
        /// Can be positive to increase the balance or negative to decrease the balance.
        /// </summary>
        [JsonProperty("value")]
        public CoinsChange Value { get; }

        /// <summary>
        /// Address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; }

        /// <summary>
        /// Optional.
        /// Tag of the address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("tag")]
        public string Tag { get; }

        /// <summary>
        /// Optional.
        /// Type of the address tag.
        /// </summary>
        [CanBeNull]
        [JsonProperty("tagType")]
        public AddressTagType? TagType { get; }

        public BalanceChange(
            string id, 
            string transferId, 
            string assetId, 
            CoinsChange value, 
            string address, 
            string tag = null, 
            AddressTagType? tagType = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Should be not empty string", nameof(id));
            
            if (string.IsNullOrWhiteSpace(transferId))
                throw new ArgumentException("Should be not empty string", nameof(transferId));

            if (string.IsNullOrWhiteSpace(assetId))
                throw new ArgumentException("Should be not empty string", nameof(assetId));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Should be not empty string", nameof(address));

            if (tag != null && string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Should be either null or not empty string", nameof(tag));

            Id = id;
            TransferId = transferId;
            AssetId = assetId;
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Address = address;
            Tag = tag;
            TagType = tagType;
        }
    }
}

using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.BlocksReader.Events
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
        public BalanceChangeId Id { get; }

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
        public AssetId AssetId { get; }

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
        public Address Address { get; }

        /// <summary>
        /// Optional.
        /// Tag of the address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("tag")]
        public AddressTag Tag { get; }

        /// <summary>
        /// Optional.
        /// Type of the address tag.
        /// </summary>
        [CanBeNull]
        [JsonProperty("tagType")]
        public AddressTagType? TagType { get; }

        /// <summary>
        /// Optional.
        /// Nonnce number of the transaction for the address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("nonce")]
        public long? Nonce { get; }

        public BalanceChange(
            BalanceChangeId id, 
            string transferId, 
            AssetId assetId, 
            CoinsChange value, 
            Address address, 
            AddressTag tag = null, 
            AddressTagType? tagType = null,
            long? nonce = null)
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
            Nonce = nonce;
        }
    }
}

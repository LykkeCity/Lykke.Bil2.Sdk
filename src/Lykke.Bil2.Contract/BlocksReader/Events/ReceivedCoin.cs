using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.JsonConverters;
using Lykke.Numerics;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    [PublicAPI]
    public class ReceivedCoin
    {
        /// <summary>
        /// Number of received coin in the coin in the transaction.
        /// </summary>
        [JsonProperty("coinNumber")]
        public int CoinNumber { get; }

        /// <summary>
        /// Asset ID of the coin.
        /// </summary>
        [JsonProperty("assetId")]
        public AssetId AssetId { get; }

        /// <summary>
        /// Value of the coin.
        /// </summary>
        [JsonProperty("value")]
        public UMoney Value { get; }

        /// <summary>
        /// Address which received the coin.
        /// </summary>
        [JsonProperty("address")]
        public Address Address { get; }

        /// <summary>
        /// Optional.
        /// Tag of the receiving address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("addressTag")]
        public AddressTag AddressTag { get; }

        /// <summary>
        /// Optional.
        /// Type of the receiving address tag.
        /// </summary>
        [CanBeNull]
        [JsonProperty("addressTagType")]
        public AddressTagType? AddressTagType { get; }
        
        /// <summary>
        /// Optional.
        /// Nonce number of the transaction for the receiving address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("addressNonce")]
        public long? AddressNonce { get; }

        public ReceivedCoin(
            int coinNumber,
            AssetId assetId,
            UMoney value,
            Address address,
            AddressTag addressTag = null,
            AddressTagType? addressTagType = null,
            long? addressNonce = null)
        {
            if (coinNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(coinNumber), coinNumber, "Should be zero or positive number");

            if (string.IsNullOrWhiteSpace(assetId))
                throw new ArgumentException("Should be not empty string", nameof(assetId));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Should be not empty string", nameof(address));

            if (addressTag != null && string.IsNullOrWhiteSpace(addressTag))
                throw new ArgumentException("Should be either null or not empty string", nameof(addressTag));

            if (addressTagType.HasValue && addressTag == null)
                throw new ArgumentException("If the tag type is specified, the tag should be specified too");

            CoinNumber = coinNumber;
            AssetId = assetId;
            Value = value;
            Address = address;
            AddressTag = addressTag;
            AddressTagType = addressTagType;
            AddressNonce = addressNonce;
        }
    }
}

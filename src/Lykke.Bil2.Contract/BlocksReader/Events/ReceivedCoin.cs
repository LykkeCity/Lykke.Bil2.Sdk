using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Numerics;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    /// <summary>
    /// Received coin.
    /// </summary>
    [PublicAPI]
    public class ReceivedCoin
    {
        /// <summary>
        /// Number of received coin in the transaction.
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
        /// Optional.
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

        /// <summary>
        /// Received coin.
        /// </summary>
        /// <param name="coinNumber">Number of received coin in the transaction.</param>
        /// <param name="assetId">Asset ID of the coin.</param>
        /// <param name="value">Value of the coin.</param>
        /// <param name="address">
        /// Optional.
        /// Address which received the coin.
        /// </param>
        /// <param name="addressTag">
        /// Optional.
        /// Tag of the receiving address.
        /// </param>
        /// <param name="addressTagType">
        /// Optional.
        /// Type of the receiving address tag.
        /// </param>
        /// <param name="addressNonce">
        /// Optional.
        /// Nonce number of the transaction for the receiving address.
        /// </param>
        public ReceivedCoin(
            int coinNumber,
            AssetId assetId,
            UMoney value,
            Address address = null,
            AddressTag addressTag = null,
            AddressTagType? addressTagType = null,
            long? addressNonce = null)
        {
            if (coinNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(coinNumber), coinNumber, "Should be zero or positive number");

            if (addressTag != null && address == null)
                throw new ArgumentException("If the tag is specified, the address should be specified too");

            if (addressTagType.HasValue && addressTag == null)
                throw new ArgumentException("If the tag type is specified, the tag should be specified too");

            CoinNumber = coinNumber;
            AssetId = assetId ?? throw new ArgumentNullException(nameof(assetId));
            Value = value;
            Address = address;
            AddressTag = addressTag;
            AddressTagType = addressTagType;
            AddressNonce = addressNonce;
        }
    }
}

using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Coin to receive for the transaction.
    /// </summary>
    [PublicAPI]
    public class CoinToReceive
    {
        /// <summary>
        /// Asset ID of the coin.
        /// </summary>
        [JsonProperty("assetId")]
        public AssetId AssetId { get; }

        /// <summary>
        /// Coin value to receive.
        /// </summary>
        [JsonProperty("value")]
        public CoinsAmount Value { get; }

        /// <summary>
        /// Address which should receive the coin.
        /// </summary>
        [JsonProperty("address")]
        public Address Address { get; }

        /// <summary>
        /// Optional.
        /// Receiving address tag.
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

        public CoinToReceive(
            AssetId assetId,
            CoinsAmount value,
            Address address,
            AddressTag addressTag = null,
            AddressTagType? addressTagType = null)
        {
            if (string.IsNullOrWhiteSpace(assetId))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(assetId));

            if(value <= 0)
                throw RequestValidationException.ShouldBePositiveNumber(value, nameof(value));

            if (string.IsNullOrWhiteSpace(address))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(address));

            if (addressTag != null && string.IsNullOrWhiteSpace(addressTag))
                throw new RequestValidationException("Should be either null or not empty string", nameof(addressTag));

            if (addressTagType.HasValue && addressTag == null)
                throw new RequestValidationException("If the tag type is specified, the tag should be specified too", new [] {nameof(addressTagType), nameof(addressTag)});

            AssetId = assetId;
            Value = value;
            Address = address;
            AddressTag = addressTag;
            AddressTagType = addressTagType;
        }
    }
}

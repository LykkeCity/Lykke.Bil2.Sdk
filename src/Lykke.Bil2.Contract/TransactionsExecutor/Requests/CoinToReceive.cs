using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;
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
        /// Number of the coin inside the transaction.
        /// </summary>
        [JsonProperty("coinNumber")]
        public int CoinNumber { get; }

        /// <summary>
        /// Asset of the coin.
        /// </summary>
        [JsonProperty("asset")]
        public Asset Asset { get; }

        /// <summary>
        /// Coin value to receive.
        /// </summary>
        [JsonProperty("value")]
        public UMoney Value { get; }

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

        /// <summary>
        /// Coin to receive for the transaction.
        /// </summary>
        /// <param name="coinNumber">Number of the coin inside the transaction.</param>
        /// <param name="asset">Asset of the coin.</param>
        /// <param name="value">Coin value to receive.</param>
        /// <param name="address">Address which should receive the coin.</param>
        /// <param name="addressTag">
        /// Optional.
        /// Receiving address tag.
        /// </param>
        /// <param name="addressTagType">
        /// Optional.
        /// Type of the receiving address tag.
        /// </param>
        public CoinToReceive(
            int coinNumber,
            Asset asset,
            UMoney value,
            Address address,
            AddressTag addressTag = null,
            AddressTagType? addressTagType = null)
        {
            if (coinNumber < 0)
                throw RequestValidationException.ShouldBeZeroOrPositiveNumber(coinNumber, nameof(coinNumber));

            if (addressTagType.HasValue && addressTag == null)
                throw new RequestValidationException("If the tag type is specified, the tag should be specified too", new [] {nameof(addressTagType), nameof(addressTag)});

            CoinNumber = coinNumber;
            Asset = asset ?? throw RequestValidationException.ShouldBeNotNull(nameof(asset));
            Value = value;
            Address = address ?? throw RequestValidationException.ShouldBeNotNull(nameof(address));
            AddressTag = addressTag;
            AddressTagType = addressTagType;
        }
    }
}

using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Coin to spend for the transaction.
    /// </summary>
    [PublicAPI]
    public class CoinToSpend
    {
        /// <summary>
        /// Id of the coin which should be spend.
        /// </summary>
        [JsonProperty("coinId")]
        public CoinId Coin { get; }

        /// <summary>
        /// Asset of the coin.
        /// </summary>
        [JsonProperty("asset")]
        public Asset Asset { get; }

        /// <summary>
        /// Coin value to spend.
        /// </summary>
        [JsonProperty("value")]
        public UMoney Value { get; }

        /// <summary>
        /// Address that owns the coin.
        /// </summary>
        [JsonProperty("address")]
        public Address Address { get; }
        
        /// <summary>
        /// Optional.
        /// Address context associated with the owner address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("addressContext")]
        public Base58String AddressContext { get; }

        /// <summary>
        /// Optional.
        /// Nonce number of the transaction for the owner address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("addressNonce")]
        public long? AddressNonce { get; }

        /// <summary>
        /// Coin to spend for the transaction.
        /// </summary>
        /// <param name="coinId">Reference to the coin which should be spend.</param>
        /// <param name="asset">Asset of the coin.</param>
        /// <param name="value">Coin value to spend.</param>
        /// <param name="address">Address that owns the coin.</param>
        /// <param name="addressContext">
        /// Optional.
        /// Address context associated with the owner address.
        /// </param>
        /// <param name="addressNonce">
        /// Optional.
        /// Nonce number of the transaction for the owner address.
        /// </param>
        public CoinToSpend(
            CoinId coinId,
            Asset asset,
            UMoney value,
            Address address,
            Base58String addressContext = null,
            long? addressNonce = null)
        {
            Coin = coinId ?? throw RequestValidationException.ShouldBeNotNull(nameof(coinId));
            Asset = asset ?? throw RequestValidationException.ShouldBeNotNull(nameof(asset));
            Value = value;
            Address = address ?? throw RequestValidationException.ShouldBeNotNull(nameof(address));
            AddressContext = addressContext;
            AddressNonce = addressNonce;
        }
    }
}

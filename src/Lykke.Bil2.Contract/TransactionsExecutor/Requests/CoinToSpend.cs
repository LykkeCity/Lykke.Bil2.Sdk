using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
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
        /// Id of the transaction within which coin was created on the address.
        /// </summary>
        [JsonProperty("transactionId")]
        public string TransactionId { get; }

        /// <summary>
        /// Number of the coin inside the transaction within which coin was created on the address.
        /// </summary>
        [JsonProperty("coinNumber")]
        public int CoinNumber { get; }

        /// <summary>
        /// Asset ID of the coin.
        /// </summary>
        [JsonProperty("assetId")]
        public AssetId AssetId { get; }

        /// <summary>
        /// Coin value to spend.
        /// </summary>
        [JsonProperty("value")]
        public CoinsAmount Value { get; }

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

        public CoinToSpend(
            string transactionId,
            int coinNumber,
            AssetId assetId,
            CoinsAmount value,
            Address address,
            Base58String addressContext = null,
            long? addressNonce = null)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(transactionId));

            if (coinNumber < 0)
                throw RequestValidationException.ShouldBeZeroOrPositiveNumber(coinNumber, nameof(coinNumber));

            if (string.IsNullOrWhiteSpace(assetId))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(assetId));

            if(value <= 0)
                throw RequestValidationException.ShouldBePositiveNumber(value, nameof(value));

            if (string.IsNullOrWhiteSpace(address))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(address));

            TransactionId = transactionId;
            CoinNumber = coinNumber;
            AssetId = assetId;
            Value = value;
            Address = address;
            AddressContext = addressContext;
            AddressNonce = addressNonce;
        }
    }
}

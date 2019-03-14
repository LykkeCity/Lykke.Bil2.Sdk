﻿using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
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
        /// Reference to the coin which should be spend.
        /// </summary>
        [JsonProperty("coin")]
        public CoinReference Coin { get; }

        /// <summary>
        /// Asset ID of the coin.
        /// </summary>
        [JsonProperty("assetId")]
        public AssetId AssetId { get; }

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
        /// <param name="coin">Reference to the coin which should be spend.</param>
        /// <param name="assetId">Asset ID of the coin.</param>
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
            CoinReference coin,
            AssetId assetId,
            UMoney value,
            Address address,
            Base58String addressContext = null,
            long? addressNonce = null)
        {
            if (string.IsNullOrWhiteSpace(assetId))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(assetId));

            if (string.IsNullOrWhiteSpace(address))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(address));

            Coin = coin ?? throw RequestValidationException.ShouldBeNotNull(nameof(coin));
            AssetId = assetId;
            Value = value;
            Address = address;
            AddressContext = addressContext;
            AddressNonce = addressNonce;
        }
    }
}

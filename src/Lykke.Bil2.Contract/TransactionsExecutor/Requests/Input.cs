using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Transaction input.
    /// </summary>
    [PublicAPI]
    public class Input
    {
        /// <summary>
        /// Asset ID to transfer.
        /// </summary>
        [JsonProperty("assetId")]
        public AssetId AssetId { get; }

        /// <summary>
        /// Address.
        /// </summary>
        [JsonProperty("address")]
        public Address Address { get; }
        
        /// <summary>
        /// Amount to transfer from the given address.
        /// </summary>
        [JsonProperty("amount")]
        public CoinsAmount Amount { get; }

        /// <summary>
        /// Optional.
        /// Address context associated with the address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("addressContext")]
        public Base58String AddressContext { get; }

        /// <summary>
        /// Optional.
        /// Nonce number of the transaction for the address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("nonce")]
        public long? Nonce { get; }

        public Input(
            AssetId assetId, 
            Address address, 
            CoinsAmount amount,
            Base58String addressContext = null,
            long? nonce = null)
        {
            if (string.IsNullOrWhiteSpace(assetId))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(assetId));

            if (string.IsNullOrWhiteSpace(address))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(address));

            if(amount <= 0)
                throw RequestValidationException.ShouldBePositiveNumber(amount, nameof(amount));

            AssetId = assetId;
            Address = address;
            Amount = amount;
            AddressContext = addressContext;
            Nonce = nonce;
        }
    }
}

using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Transfer of the transaction.
    /// </summary>
    [PublicAPI]
    public class Transfer
    {
        /// <summary>
        /// Asset ID to transfer.
        /// </summary>
        [JsonProperty("assetId")]
        public AssetId AssetId { get; }

        /// <summary>
        /// Amount to transfer from the source address
        /// to the destination address.
        /// </summary>
        [JsonProperty("amount")]
        public CoinsAmount Amount { get; }
        
        /// <summary>
        /// Address to transfer from.
        /// </summary>
        [JsonProperty("sourceAddress")]
        public Address SourceAddress { get; }
        
        /// <summary>
        /// Address to transfer to.
        /// </summary>
        [JsonProperty("destinationAddress")]
        public Address DestinationAddress { get; }
        
        /// <summary>
        /// Optional.
        /// Source address context associated with the address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("sourceAddressContext")]
        public Base58String SourceAddressContext { get; }
        
        /// <summary>
        /// Optional.
        /// Nonce number of the transaction for the source address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("sourceAddressNonce")]
        public long? SourceAddressNonce { get; }

        /// <summary>
        /// Optional.
        /// Destination address tag.
        /// </summary>
        [CanBeNull]
        [JsonProperty("destinationAddressTag")]
        public AddressTag DestinationAddressTag { get; }

        /// <summary>
        /// Optional.
        /// Type of the destination address tag.
        /// </summary>
        [CanBeNull]
        [JsonProperty("destinationAddressTagType")]
        public AddressTagType? DestinationAddressTagType { get; }

        /// <summary>
        /// Transfer of the transaction.
        /// </summary>
        public Transfer(
            AssetId assetId,
            CoinsAmount amount,
            Address sourceAddress,
            Address destinationAddress,
            Base58String sourceAddressContext = null,
            long? sourceAddressNonce = null,
            AddressTag destinationAddressTag = null,
            AddressTagType? destinationAddressTagType = null)
        {
            if (string.IsNullOrWhiteSpace(assetId))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(assetId));

            if(amount <= 0)
                throw RequestValidationException.ShouldBePositiveNumber(amount, nameof(amount));

            if (string.IsNullOrWhiteSpace(sourceAddress))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(sourceAddress));

            if (string.IsNullOrWhiteSpace(destinationAddress))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(destinationAddress));

            if (destinationAddressTagType.HasValue && destinationAddressTag == null)
                throw new RequestValidationException("If the tag type is specified, the tag should be specified too", new [] {nameof(destinationAddressTagType), nameof(destinationAddressTag)});

            AssetId = assetId;
            Amount = amount;
            SourceAddress = sourceAddress;
            SourceAddressContext = sourceAddressContext;
            SourceAddressNonce = sourceAddressNonce;
            DestinationAddress = destinationAddress;
            DestinationAddressTag = destinationAddressTag;
            DestinationAddressTagType = destinationAddressTagType;
        }
    }
}

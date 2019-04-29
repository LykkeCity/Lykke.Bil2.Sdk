using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;
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
        /// Asset to transfer.
        /// </summary>
        [JsonProperty("asset")]
        public Asset Asset { get; }

        /// <summary>
        /// Amount to transfer from the source address
        /// to the destination address.
        /// </summary>
        [JsonProperty("amount")]
        public UMoney Amount { get; }
        
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
        public Base64String SourceAddressContext { get; }
        
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
        /// <param name="asset">Asset to transfer.</param>
        /// <param name="amount">Amount to transfer from the source address to the destination address.</param>
        /// <param name="sourceAddress">Address to transfer from.</param>
        /// <param name="destinationAddress">Address to transfer to.</param>
        /// <param name="sourceAddressContext">
        /// Optional.
        /// Source address context associated with the address.
        /// </param>
        /// <param name="sourceAddressNonce">
        /// Optional.
        /// Nonce number of the transaction for the source address.
        /// </param>
        /// <param name="destinationAddressTag">
        /// Optional.
        /// Destination address tag.
        /// </param>
        /// <param name="destinationAddressTagType">
        /// Optional.
        /// Type of the destination address tag.
        /// </param>
        public Transfer(
            Asset asset,
            UMoney amount,
            Address sourceAddress,
            Address destinationAddress,
            Base64String sourceAddressContext = null,
            long? sourceAddressNonce = null,
            AddressTag destinationAddressTag = null,
            AddressTagType? destinationAddressTagType = null)
        {           
            if (destinationAddressTagType.HasValue && destinationAddressTag == null)
                throw new RequestValidationException("If the tag type is specified, the tag should be specified too", new [] {nameof(destinationAddressTagType), nameof(destinationAddressTag)});

            Asset = asset ?? throw RequestValidationException.ShouldBeNotNull(nameof(asset));
            Amount = amount;
            SourceAddress = sourceAddress ?? throw RequestValidationException.ShouldBeNotNull(nameof(sourceAddress));
            SourceAddressContext = sourceAddressContext;
            SourceAddressNonce = sourceAddressNonce;
            DestinationAddress = destinationAddress ?? throw RequestValidationException.ShouldBeNotNull(nameof(destinationAddress));
            DestinationAddressTag = destinationAddressTag;
            DestinationAddressTagType = destinationAddressTagType;
        }
    }
}

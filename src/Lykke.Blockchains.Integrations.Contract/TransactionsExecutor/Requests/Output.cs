using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Transaction output.
    /// </summary>
    [PublicAPI]
    public class Output
    {
        /// <summary>
        /// Asset ID to transfer.
        /// </summary>
        [JsonProperty("assetId")]
        public string AssetId { get; }

        /// <summary>
        /// Address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; }

        /// <summary>
        /// Amount to transfer to the given address.
        /// </summary>
        [JsonProperty("amount")]
        public CoinsAmount Amount { get; }

        /// <summary>
        /// Optional.
        /// Address tag.
        /// </summary>
        [CanBeNull]
        [JsonProperty("tag")]
        public string Tag { get; }

        /// <summary>
        /// Optional.
        /// Type of the address tag.
        /// </summary>
        [CanBeNull]
        [JsonProperty("tagType")]
        public AddressTagType? TagType { get; }

        public Output(
            string assetId,
            string address,
            CoinsAmount amount,
            string tag = null,
            AddressTagType? tagType = null)
        {
            if (string.IsNullOrWhiteSpace(assetId))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(assetId));

            if (string.IsNullOrWhiteSpace(address))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(address));

            if(amount <= 0)
                throw RequestValidationException.ShouldBePositiveNumber(amount, nameof(amount));

            if (tagType.HasValue && tag == null)
                throw new RequestValidationException("If the tag type is specified, the tag should be specified too", new [] {nameof(tagType), nameof(tag)});

            AssetId = assetId;
            Address = address;
            Amount = amount;
            Tag = tag;
            TagType = tagType;
        }
    }
}

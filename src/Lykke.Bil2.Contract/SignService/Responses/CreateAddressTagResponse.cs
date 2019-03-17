using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.SignService.Responses
{
    /// <summary>
    /// Endpoint: [POST] /api/addresses/{address}/tags
    /// </summary>
    [PublicAPI]
    public class CreateAddressTagResponse
    {
        /// <summary>
        /// Generated address tag.
        /// </summary>
        [JsonProperty("tag")]
        public AddressTag Tag { get; }

        /// <summary>
        /// Optional.
        /// Any non security sensitive, implementation specific information associated with the address tag.
        /// </summary>
        [CanBeNull]
        [JsonProperty("tagContext")]
        public Base58String TagContext { get; }

        /// <summary>
        /// Endpoint: [POST] /api/addresses/{address}/tags
        /// </summary>
        /// <param name="tag">Generated address tag.</param>
        /// <param name="tagContext">
        /// Optional.
        /// Any non security sensitive, implementation specific information associated with the address tag.
        /// </param>
        public CreateAddressTagResponse(AddressTag tag, Base58String tagContext = null)
        {
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            TagContext = tagContext;
        }
    }
}

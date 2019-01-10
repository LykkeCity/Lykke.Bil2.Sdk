using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.SignService.Responses
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
        public string Tag { get; set; }

        /// <summary>
        /// Optional.
        /// Any non security sensitive, implementation specific information associated with the address tag.
        /// </summary>
        [CanBeNull]
        [JsonProperty("tagContext")]
        public Base64String TagContext { get; set; }
    }
}

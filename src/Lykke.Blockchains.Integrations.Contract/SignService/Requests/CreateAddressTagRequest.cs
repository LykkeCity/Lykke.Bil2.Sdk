using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lykke.Blockchains.Integrations.Contract.SignService.Requests
{
    /// <summary>
    /// Endpoint: [POST] /api/addresses/{address}/tags
    /// </summary>
    [PublicAPI]
    public class CreateAddressTagRequest
    {      
        /// <summary>
        /// Optional.
        /// Implementation specific address context associated with the address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("addressContext")]
        public Base64String AddressContext { get; set; }

        /// <summary>
        /// Optional.
        /// Type of the address tag being created. Actual value depends on implementation.
        /// </summary>
        [CanBeNull]
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy), new object[0], false)]
        public AddressTagType? Type { get; set; }
    }
}

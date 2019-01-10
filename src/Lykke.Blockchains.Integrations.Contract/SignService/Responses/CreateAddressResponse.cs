using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.SignService.Responses
{
    /// <summary>
    /// Endpoint: [POST] /api/addresses
    /// </summary>
    [PublicAPI]
    public class CreateAddressResponse
    {
        /// <summary>
        /// Encrypted private key of the address.
        /// </summary>
        [JsonProperty("encryptedPrivateKey")]
        public string EncryptedPrivateKey { get; set; }
        
        /// <summary>
        /// Generated address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Optional.
        /// Any non security sensitive, implementation specific information associated with the address. 
        /// </summary>
        [CanBeNull]
        [JsonProperty("addressContext")]
        public Base64String AddressContext { get; set; }
    }
}

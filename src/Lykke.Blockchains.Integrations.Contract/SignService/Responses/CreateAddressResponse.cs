using System;
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
        public string EncryptedPrivateKey { get; }
        
        /// <summary>
        /// Generated address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; }

        /// <summary>
        /// Optional.
        /// Any non security sensitive, implementation specific information associated with the address. 
        /// </summary>
        [CanBeNull]
        [JsonProperty("addressContext")]
        public Base64String AddressContext { get; }

        public CreateAddressResponse(string encryptedPrivateKey, string address, Base64String addressContext = null)
        {
            if (string.IsNullOrWhiteSpace(encryptedPrivateKey))
                throw new ArgumentException("Should be not empty string", nameof(encryptedPrivateKey));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Should be not empty string", nameof(address));

            EncryptedPrivateKey = encryptedPrivateKey;
            Address = address;
            AddressContext = addressContext;
        }
    }
}

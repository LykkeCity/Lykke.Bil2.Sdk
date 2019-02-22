using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.SignService.Responses
{
    /// <summary>
    /// Endpoint: [POST] /api/addresses
    /// </summary>
    [PublicAPI]
    public class CreateAddressResponse
    {
        /// <summary>
        /// Private key of the address.
        /// </summary>
        [JsonProperty("privateKey")]
        public EncryptedString PrivateKey { get; }
        
        /// <summary>
        /// Generated address.
        /// </summary>
        [JsonProperty("address")]
        public Address Address { get; }

        /// <summary>
        /// Optional.
        /// Any non security sensitive, implementation specific information associated with the address. 
        /// </summary>
        [CanBeNull]
        [JsonProperty("addressContext")]
        public Base58String AddressContext { get; }

        public CreateAddressResponse(EncryptedString privateKey, Address address, Base58String addressContext = null)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Should be not empty string", nameof(address));

            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            Address = address;
            AddressContext = addressContext;
        }
    }
}

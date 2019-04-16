using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.SharedDomain;
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

        /// <summary>
        /// Endpoint: [POST] /api/addresses
        /// </summary>
        /// <param name="privateKey">Private key of the address.</param>
        /// <param name="address">Generated address.</param>
        /// <param name="addressContext">
        /// Optional.
        /// Any non security sensitive, implementation specific information associated with the address. 
        /// </param>
        public CreateAddressResponse(EncryptedString privateKey, Address address, Base58String addressContext = null)
        {
            PrivateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            AddressContext = addressContext;
        }
    }
}

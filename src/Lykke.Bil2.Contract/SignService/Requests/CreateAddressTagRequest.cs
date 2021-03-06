﻿using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.SignService.Requests
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
        public AddressTagType? Type { get; set; }

        /// <summary>
        /// Endpoint: [POST] /api/addresses/{address}/tags
        /// </summary>
        /// <param name="addressContext">
        /// Optional.
        /// Implementation specific address context associated with the address.
        /// </param>
        /// <param name="type">
        /// Optional.
        /// Type of the address tag being created. Actual value depends on implementation.
        /// </param>
        public CreateAddressTagRequest(Base64String addressContext = null, AddressTagType? type = null)
        {
            AddressContext = addressContext;
            Type = type;
        }
    }
}

using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Particular format of an address.
    /// </summary>
    [PublicAPI]
    public class AddressFormat
    {
        /// <summary>
        /// Address in the particular format.
        /// </summary>
        [JsonProperty("address")]
        public Address Address { get; }

        /// <summary>
        /// Optional.
        /// Name of the format. Can be omitted for one item in the list.
        /// Omitted value will be interpreted as default address format.
        /// </summary>
        [JsonProperty("formatName")]
        public string FormatName { get; }

        public AddressFormat(Address address, string formatName = null)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Should be not empty string", nameof(address));

            if (formatName != null && string.IsNullOrWhiteSpace(formatName))
                throw new ArgumentException("Should be either null or not empty string", nameof(formatName));

            Address = address;
            FormatName = formatName;
        }
    }
}

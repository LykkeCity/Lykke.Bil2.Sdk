using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lykke.Bil2.Contract.TransactionsExecutor
{
    /// <summary>
    /// Enum describing result of the address validation
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy), new object[0], false)]
    public enum AddressValidationResult
    {
        /// <summary>
        /// Address is valid.
        /// </summary>
        [EnumMember(Value = "valid")]
        Valid,

        /// <summary>
        /// address format is invalid.
        /// </summary>
        [EnumMember(Value = "invalidAddressFormat")]
        InvalidAddressFormat,

        /// <summary>
        /// Address is not found.
        /// </summary>
        [EnumMember(Value = "addressNotFound")]
        AddressNotFound,

        /// <summary>
        /// Address tag format is invalid.
        /// </summary>
        [EnumMember(Value = "invalidTagFormat")]
        InvalidTagFormat,
        
        /// <summary>
        /// Address requires a tag, but it isn’t specified.
        /// </summary>
        [EnumMember(Value = "requiredTagMissed")]
        RequiredTagMissed,

        /// <summary>
        /// Address type is not supported. For instance address is a contract and it’s not supported.
        /// </summary>
        [EnumMember(Value = "addressTypeNotSupported ")]
        AddressTypeNotSupported 
    }
}

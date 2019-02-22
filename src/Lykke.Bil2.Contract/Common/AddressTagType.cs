using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lykke.Bil2.Contract.Common
{
    /// <summary>
    /// Enum describing implementation specific type of the address tag.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy), new object[0], false)]
    public enum AddressTagType
    {
        /// <summary>
        /// Address tag as a number.
        /// </summary>
        [EnumMember(Value = "number")]
        Number,

        /// <summary>
        /// Address tag as a text.
        /// </summary>
        [EnumMember(Value = "text")]
        Text
    }
}

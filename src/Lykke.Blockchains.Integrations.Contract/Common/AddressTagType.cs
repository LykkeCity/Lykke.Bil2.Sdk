using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    /// <summary>
    /// Enum describing implementation specific type of the address tag.
    /// </summary>
    [PublicAPI]
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

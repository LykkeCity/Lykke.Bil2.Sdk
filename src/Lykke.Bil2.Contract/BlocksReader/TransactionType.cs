using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lykke.Bil2.Contract.BlocksReader
{
    /// <summary>
    /// Enum describing implementation specific type of the transaction.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy), new object[0], false)]
    public enum TransactionType
    {
        /// <summary>
        /// Corresponds to the “sending” transaction, for example in Nano blockchain.
        /// </summary>
        [EnumMember(Value = "sending")]
        Sending,

        /// <summary>
        /// Corresponds to the “receiving” transaction, for example in Nano blockchain.
        /// </summary>
        [EnumMember(Value = "receiving ")]
        Receiving
    }
}

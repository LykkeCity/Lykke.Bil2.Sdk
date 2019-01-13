using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor
{
    /// <summary>
    /// Enum describing reason of the transaction bulding failure.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy), new object[0], false)]
    public enum ReceivingTransactionBuildingError
    {
        /// <summary>
        /// There is a temporary issue with infrastructure or configuration.
        /// </summary>
        [EnumMember(Value = "transientFailure")]
        TransientFailure ,

        /// <summary>
        /// Transaction can’t be built with the given parameters. The given “sending” transaction
        /// can’t be received and it will be never possible to receive the given “sending” transaction.
        /// </summary>
        [EnumMember(Value = "rejected")]
        Rejected
    }
}

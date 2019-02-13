using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor
{
    /// <summary>
    /// Enum describing the type of the fee.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy), new object[0], false)]
    public enum FeeType
    {
        /// <summary>
        /// Fee should be deducted from the transaction amount, so total amount of the transaction
        /// including the fee should be equal to the initial transaction amount.
        /// </summary>
        [EnumMember(Value = "deductFromAmount")]
        DeductFromAmount,

        /// <summary>
        /// fee should be added to the transaction amount, so total amount of the transaction
        /// including the fee should be equal to the initial transaction amount plus the fee amount.
        /// </summary>
        [EnumMember(Value = "addToAmount")]
        AddToAmount 
    }
}

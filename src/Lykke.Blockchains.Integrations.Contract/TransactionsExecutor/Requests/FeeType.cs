using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Enum describing the type of the fee.
    /// </summary>
    [PublicAPI]
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

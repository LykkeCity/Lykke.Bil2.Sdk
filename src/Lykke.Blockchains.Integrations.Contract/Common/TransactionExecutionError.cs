using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    /// <summary>
    /// Enum describing reason of the transaction execution failure.
    /// </summary>
    [PublicAPI]
    public enum TransactionExecutionError
    {
        /// <summary>
        /// Any not well known errors. It’s not guaranteed if transaction was
        /// broadcasted to the blockchain or not.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown,

        /// <summary>
        /// Transaction can’t be broadcasted with the given parameters.
        /// It should be guaranteed that the transaction is not included
        /// and will not be included to the any blockchain block.
        /// </summary>
        [EnumMember(Value = "rejected")]
        Rejected,
        
        /// <summary>
        /// There is not enough balance on the some of the input address.
        /// It should be guaranteed that the transaction is not included
        /// and will not be included to the any blockchain block.
        /// </summary>
        [EnumMember(Value = "notEnoughBalance")]
        NotEnoughBalance,
        
        /// <summary>
        /// Transaction should be built again. It should be guaranteed that the
        /// transaction is not included and will not be included to the any blockchain block.
        /// </summary>
        [EnumMember(Value = "rebuildRequired")]
        RebuildRequired,

        /// <summary>
        /// There is a temporary issue with infrastructure or configuration.
        /// It should be guaranteed that the transaction is not included
        /// and will not be included to the any blockchain block.
        /// </summary>
        [EnumMember(Value = "transientFailure")]
        TransientFailure
    }
}
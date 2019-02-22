using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Lykke.Bil2.Contract.TransactionsExecutor
{
    /// <summary>
    /// Enum describing reason of the transaction broadcasting failure.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy), new object[0], false)]
    public enum TransactionBroadcastingError
    {
        /// <summary>
        /// There is not enough balance on the some of the input address.
        /// It should be guaranteed that the transaction is not included
        /// and will not be included to the any blockchain block.
        /// </summary>
        [EnumMember(Value = "notEnoughBalance")]
        NotEnoughBalance,

        /// <summary>
        /// Fee amount is not enough to execute the transaction.
        /// It should be guaranteed that the transaction is not included and
        /// will not be included to the any blockchain block.
        /// </summary>
        [EnumMember(Value = "feeTooLow")]
        FeeTooLow,
        
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
        TransientFailure,

        /// <summary>
        /// Integration can’t serve request right now due to workload or any other reason.
        /// Request should be retried later. It should be guaranteed that the transaction
        /// is not included and will not be included to the any blockchain block.
        /// </summary>
        [EnumMember(Value = "retryLater")]
        RetryLater 
    }
}

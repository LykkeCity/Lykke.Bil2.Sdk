﻿using System.Runtime.Serialization;
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
    public enum SendingTransactionBuildingError
    {
        /// <summary>
        /// There is a temporary issue with infrastructure or configuration.
        /// </summary>
        [EnumMember(Value = "transientFailure")]
        TransientFailure ,

        /// <summary>
        /// Transaction can’t be built with the given parameters and it will be never
        /// possible to build the transaction with exactly the same parameters.
        /// </summary>
        [EnumMember(Value = "rejected")]
        Rejected,
        
        /// <summary>
        /// There is not enough balance on the some of the input address.
        /// Transaction can be rebuilt later with exactly the same parameters or
        /// transfered amount, including fee, should be reduced.
        /// </summary>
        [EnumMember(Value = "notEnoughBalance")]
        NotEnoughBalance,

        /// <summary>
        /// Fee amount is not enough to execute the transaction.
        /// Fee should be increased and transaction should be built again.
        /// </summary>
        [EnumMember(Value = "feeTooLow")]
        FeeTooLow
    }
}

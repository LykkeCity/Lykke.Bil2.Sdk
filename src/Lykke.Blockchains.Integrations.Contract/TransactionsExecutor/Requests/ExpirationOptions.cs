using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Transaction expiration options.
    /// </summary>
    [PublicAPI]
    public class ExpirationOptions
    {
        /// <summary>
        /// Transaction should be expired after given moment.
        /// </summary>
        [CanBeNull]
        [JsonProperty("afterMoment")]
        public DateTime? AfterMoment { get; set; }

        /// <summary>
        /// Transaction should be expired after given block number.
        /// </summary>
        [CanBeNull]
        [JsonProperty("afterBlockNumber")]
        public long? AfterBlockNumber { get; set; }

    }
}

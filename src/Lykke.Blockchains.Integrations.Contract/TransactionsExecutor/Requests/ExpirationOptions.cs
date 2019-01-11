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
        public DateTime? AfterMoment { get; }

        /// <summary>
        /// Transaction should be expired after given block number.
        /// </summary>
        [CanBeNull]
        [JsonProperty("afterBlockNumber")]
        public long? AfterBlockNumber { get; }

        public ExpirationOptions(DateTime? afterMoment = null, long? afterBlockNumber = null)
        {
            if (!afterMoment.HasValue && !afterBlockNumber.HasValue)
                throw new ArgumentException("At least one option should be specified");

            if (afterBlockNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(afterBlockNumber), afterBlockNumber, "Should be positive number");

            AfterMoment = afterMoment;
            AfterBlockNumber = afterBlockNumber;
        }

    }
}

using System;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
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
                throw new RequestValidationException("At least one option should be specified", new []{nameof(afterMoment), nameof(afterBlockNumber)});

            if (afterBlockNumber < 1)
                throw RequestValidationException.ShouldBePositiveNumber(afterBlockNumber, nameof(afterBlockNumber));

            AfterMoment = afterMoment;
            AfterBlockNumber = afterBlockNumber;
        }

    }
}

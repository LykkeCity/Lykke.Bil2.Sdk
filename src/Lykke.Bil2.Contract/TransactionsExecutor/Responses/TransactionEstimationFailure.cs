using System;
using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Responses
{
    /// <summary>
    /// Object describing an failure of a transaction estimation
    /// if it can be mapped to the <see cref="TransactionEstimationError"/>.
    /// </summary>
    [PublicAPI]
    public class TransactionEstimationFailure
    {
        /// <summary>
        /// Error code.
        /// </summary>
        [JsonProperty("code")]
        public TransactionEstimationError Code { get; }
        
        /// <summary>
        /// Clean error description.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; }

        /// <summary>
        /// Object describing an failure of a transaction estimation
        /// if it can be mapped to the <see cref="TransactionEstimationError"/>.
        /// </summary>
        /// <param name="code">Error code.</param>
        /// <param name="message">Clean error description.</param>
        public TransactionEstimationFailure(TransactionEstimationError code, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Should be not empty string", nameof(message));
            }

            Code = code;
            Message = message;
        }
    }
}
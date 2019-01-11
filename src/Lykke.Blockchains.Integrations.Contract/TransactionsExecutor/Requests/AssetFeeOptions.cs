using System;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Transaction fee options for particular asset
    /// </summary>
    public class AssetFeeOptions
    {
        /// <summary>
        /// Optional.
        /// Maximum fee amount to spend for the given transaction. 
        /// </summary>
        [CanBeNull]
        [JsonProperty("limit")]
        public CoinsAmount Limit { get; }

        /// <summary>
        /// Optional.
        /// Exact fee amount to spend for the given transaction.
        /// </summary>
        [CanBeNull]
        [JsonProperty("exact")]
        public CoinsAmount Exact { get; }

        /// <summary>
        /// Optional.
        /// Multiplier for the calculated fee amount for the given transaction.
        /// </summary>
        [CanBeNull]
        [JsonProperty("multiplier")]
        public decimal? Multiplier { get; }

        public AssetFeeOptions(CoinsAmount limit, CoinsAmount exact, decimal? multiplier)
        {
            if (limit == null && exact == null && !multiplier.HasValue)
                throw new ArgumentException("At least one option should be specified");

            if (limit != null && exact != null)
                throw new ArgumentException("Only one of limit or exact values can be specified");

            if (multiplier.HasValue && exact != null)
                throw new ArgumentException("Only one of multipler or exact values can be specified");

            if (limit <= 0)
                throw new ArgumentOutOfRangeException(nameof(limit), "Should be positive number");

            if (exact <= 0)
                throw new ArgumentOutOfRangeException(nameof(limit), "Should be positive number");

            if (multiplier <= 0)
                throw new ArgumentOutOfRangeException(nameof(limit), "Should be positive number");

            Limit = limit;
            Exact = exact;
            Multiplier = multiplier;
        }
    }
}

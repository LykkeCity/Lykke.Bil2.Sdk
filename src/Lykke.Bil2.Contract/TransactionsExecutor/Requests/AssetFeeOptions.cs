using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
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

        /// <summary>
        /// Transaction fee options for particular asset
        /// </summary>
        public AssetFeeOptions(CoinsAmount limit, CoinsAmount exact, decimal? multiplier)
        {
            if (limit == null && exact == null && !multiplier.HasValue)
                throw new RequestValidationException("At least one option should be specified", new[] {nameof(limit), nameof(exact), nameof(multiplier)});

            if (limit != null && exact != null)
                throw new RequestValidationException($"Only one of limit or exact values can be specified. Limit: {limit}, exact: {exact}", new [] {nameof(limit), nameof(exact)});

            if (multiplier.HasValue && exact != null)
                throw new RequestValidationException($"Only one of multiplier or exact values can be specified. Multiplier: {multiplier}, exact: {exact}", new [] {nameof(multiplier), nameof(exact)});

            if (limit <= 0)
                throw RequestValidationException.ShouldBePositiveNumber(limit, nameof(limit));

            if (exact <= 0)
                throw RequestValidationException.ShouldBePositiveNumber(exact, nameof(exact));

            if (multiplier <= 0)
                throw RequestValidationException.ShouldBePositiveNumber(multiplier, nameof(multiplier));

            Limit = limit;
            Exact = exact;
            Multiplier = multiplier;
        }
    }
}

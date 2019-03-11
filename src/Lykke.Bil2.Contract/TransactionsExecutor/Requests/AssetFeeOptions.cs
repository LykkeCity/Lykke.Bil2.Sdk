﻿using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.Common.JsonConverters;
using Lykke.Numerics.Money;
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
        [JsonConverter(typeof(UMoneyJsonConverter))]
        public UMoney? Limit { get; }

        /// <summary>
        /// Optional.
        /// Exact fee amount to spend for the given transaction.
        /// </summary>
        [CanBeNull]
        [JsonProperty("exact")]
        [JsonConverter(typeof(UMoneyJsonConverter))]
        public UMoney? Exact { get; }

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
        public AssetFeeOptions(UMoney? limit, UMoney? exact, decimal? multiplier)
        {
            if (!limit.HasValue && !exact.HasValue && !multiplier.HasValue)
                throw new RequestValidationException("At least one option should be specified", new[] {nameof(limit), nameof(exact), nameof(multiplier)});

            if (limit.HasValue && exact.HasValue)
                throw new RequestValidationException($"Only one of limit or exact values can be specified. Limit: {limit}, exact: {exact}", new [] {nameof(limit), nameof(exact)});

            if (multiplier.HasValue && exact.HasValue)
                throw new RequestValidationException($"Only one of multiplier or exact values can be specified. Multiplier: {multiplier}, exact: {exact}", new [] {nameof(multiplier), nameof(exact)});

            if (multiplier <= 0)
                throw RequestValidationException.ShouldBePositiveNumber(multiplier, nameof(multiplier));

            Limit = limit;
            Exact = exact;
            Multiplier = multiplier;
        }
    }
}

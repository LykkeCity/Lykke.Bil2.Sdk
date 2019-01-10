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
        public CoinsAmount Limit { get; set; }

        /// <summary>
        /// Optional.
        /// Exact fee amount to spend for the given transaction.
        /// </summary>
        [CanBeNull]
        [JsonProperty("exact")]
        public CoinsAmount Exact { get; set; }

        /// <summary>
        /// Optional.
        /// Multiplier for the calculated fee amount for the given transaction.
        /// </summary>
        [CanBeNull]
        [JsonProperty("multiplier")]
        public decimal? Multiplier { get; set; }
    }
}

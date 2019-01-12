using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Transaction fee options
    /// </summary>
    [PublicAPI]
    public class FeeOptions
    {
        /// <summary>
        /// Type of the fee.
        /// </summary>
        [JsonProperty("type")]
        public FeeType Type { get; }

        /// <summary>
        /// Optional.
        /// Fee options for particular asset ID.
        /// </summary>
        [CanBeNull]
        [JsonProperty("assetOptions")]
        public IDictionary<string, AssetFeeOptions> AssetOptions { get; }

        public FeeOptions(FeeType type, IDictionary<string, AssetFeeOptions> assetOptions = null)
        {
            Type = type;
            AssetOptions = assetOptions;
        }
    }
}

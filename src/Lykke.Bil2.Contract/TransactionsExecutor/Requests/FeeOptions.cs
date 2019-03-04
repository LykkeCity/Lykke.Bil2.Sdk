using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Transaction fee options
    /// </summary>
    [PublicAPI]
    public class FeeOptions
    {
        /// <summary>
        /// Optional.
        /// Fee options for particular asset ID.
        /// </summary>
        [CanBeNull]
        [JsonProperty("assetOptions")]
        public IReadOnlyDictionary<AssetId, AssetFeeOptions> AssetOptions { get; }

        /// <summary>
        /// Transaction fee options
        /// </summary>
        public FeeOptions(IReadOnlyDictionary<AssetId, AssetFeeOptions> assetOptions = null)
        {
            AssetOptions = assetOptions;
        }
    }
}

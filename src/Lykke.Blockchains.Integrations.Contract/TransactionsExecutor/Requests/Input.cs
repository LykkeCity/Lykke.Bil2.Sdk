using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Transaction input.
    /// </summary>
    [PublicAPI]
    public class Input
    {
        /// <summary>
        /// Asset ID to transfer.
        /// </summary>
        [JsonProperty("assetId")]
        public string AssetId { get;set; }

        /// <summary>
        /// Address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Optional.
        /// Address context associated with the address.
        /// </summary>
        [CanBeNull]
        [JsonProperty("addressContext")]
        public string AddressContext { get; set; }

        /// <summary>
        /// Amount to transfer from the given address.
        /// </summary>
        [JsonProperty("amount")]
        public CoinsAmount Amount { get; set; }
    }
}

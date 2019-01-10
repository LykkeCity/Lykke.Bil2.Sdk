using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
{

    /// <summary>
    /// Endpoint: [POST] /api/transactions/sending/built
    /// </summary>
    [PublicAPI]
    public class BuildSendingTransactionRequest
    {
        /// <summary>
        /// Transaction inputs.
        /// </summary>
        [JsonProperty("inputs")]
        public Input[] Inputs { get; set; }

        /// <summary>
        /// Transaction outputs.
        /// </summary>
        [JsonProperty("outputs")]
        public Output[] Outputs { get; set; }

        /// <summary>
        /// Fee options.
        /// </summary>
        [JsonProperty("fee")]
        public FeeOptions Fee { get; set; }

        /// <summary>
        /// Optional.
        /// Transaction expiration options. If omitted and
        /// blockchain requires transaction expiration to be
        /// specified, default value for the blockchain/integration 
        /// should be used. If several expiration options are
        /// specified at once, and blockchain supports
        /// them, then transaction should be expired when earliest
        /// condition is triggered.
        /// </summary>
        [CanBeNull]
        [JsonProperty("expiration")]
        public ExpirationOptions Expiration { get; set; }
    }
}

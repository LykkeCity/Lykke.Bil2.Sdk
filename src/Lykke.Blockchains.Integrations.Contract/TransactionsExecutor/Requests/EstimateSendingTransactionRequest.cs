using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/sending/estimated
    /// </summary>
    public class EstimateSendingTransactionRequest
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
    }
}

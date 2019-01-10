using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/receiving/built
    /// </summary>
    public class BuildReceivingTransactionRequest
    {
        /// <summary>
        /// Hash of the “sending” transaction.
        /// </summary>
        [JsonProperty("sendingTransactionHash")]
        public string SendingTransactionHash { get; set; }

        /// <summary>
        /// Address which wants to receive the “sending” transaction.
        /// </summary>
        [JsonProperty("receivingAddress")]
        public string ReceivingAddress { get; set; }
    }
}

using Lykke.Blockchains.Integrations.Contract.Common;
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
        public string SendingTransactionHash { get; }

        /// <summary>
        /// Address which wants to receive the “sending” transaction.
        /// </summary>
        [JsonProperty("receivingAddress")]
        public string ReceivingAddress { get; }

        public BuildReceivingTransactionRequest(string sendingTransactionHash, string receivingAddress)
        {
            if (string.IsNullOrWhiteSpace(sendingTransactionHash))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(sendingTransactionHash));

            if (string.IsNullOrWhiteSpace(receivingAddress))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(receivingAddress));

            SendingTransactionHash = sendingTransactionHash;
            ReceivingAddress = receivingAddress;
        }
    }
}

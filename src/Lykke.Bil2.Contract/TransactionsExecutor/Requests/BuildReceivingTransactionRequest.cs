using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
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
        public Address ReceivingAddress { get; }

        /// <summary>
        /// Endpoint: [POST] /api/transactions/receiving/built
        /// </summary>
        public BuildReceivingTransactionRequest(string sendingTransactionHash, Address receivingAddress)
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

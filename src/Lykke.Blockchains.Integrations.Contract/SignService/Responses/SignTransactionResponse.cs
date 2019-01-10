using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.SignService.Responses
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/signed
    /// </summary>
    [PublicAPI]
    public class SignTransactionResponse
    {
        /// <summary>
        /// Implementation specific signed transaction.
        /// </summary>
        [JsonProperty("signedTransaction")]
        public Base64String SignedTransaction { get; set; }

        /// <summary>
        /// Hash of the signed transaction in the blockchain.
        /// </summary>
        [JsonProperty("transactionHash")]
        public string TransactionHash { get; set; }
    }
}
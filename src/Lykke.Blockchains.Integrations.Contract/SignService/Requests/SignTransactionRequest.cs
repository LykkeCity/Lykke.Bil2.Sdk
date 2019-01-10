using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.SignService.Requests
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/signed
    /// </summary>
    [PublicAPI]
    public class SignTransactionRequest
    {
        /// <summary>
        /// Private keys of the addresses which should sign the transaction.
        /// Multiple keys can be used for the/ transactions with multiple inputs.
        /// </summary>
        [JsonProperty("privateKeys")]
        public ICollection<string> PrivateKeys { get; set; }

        /// <summary>
        /// Implementation specific transaction context.
        /// </summary>
        [JsonProperty("transactionContext")]
        public Base64String TransactionContext { get; set; }
    }
}
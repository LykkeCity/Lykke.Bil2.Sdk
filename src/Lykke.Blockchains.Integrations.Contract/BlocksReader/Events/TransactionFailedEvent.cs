using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.BlocksReader.Events
{
    /// <summary>
    /// Should be published for each failed transaction in the block being read.
    /// </summary>
    [PublicAPI]
    public class TransactionFailedEvent
    {
        /// <summary>
        /// Hash of the block.
        /// </summary>
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        /// <summary>
        /// Hash of the transaction.
        /// </summary>
        [JsonProperty("transactionHash")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// Code of the error.
        /// </summary>
        [JsonProperty("errorCode")]
        public TransactionExecutionError ErrorCode { get; set; }

        /// <summary>
        /// Clean error description.
        /// </summary>
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}

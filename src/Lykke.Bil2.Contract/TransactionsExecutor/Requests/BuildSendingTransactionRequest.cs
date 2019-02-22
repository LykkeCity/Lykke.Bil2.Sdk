using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
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
        public ICollection<Input> Inputs { get; }

        /// <summary>
        /// Transaction outputs.
        /// </summary>
        [JsonProperty("outputs")]
        public ICollection<Output> Outputs { get; }

        /// <summary>
        /// Fee options.
        /// </summary>
        [JsonProperty("fee")]
        public FeeOptions Fee { get; }

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
        public ExpirationOptions Expiration { get; }

        public BuildSendingTransactionRequest(
            ICollection<Input> inputs, 
            ICollection<Output> outputs, 
            FeeOptions fee, 
            ExpirationOptions expiration = null)
        {
            SendingTransactionInputsOutputsValidator.Validate(inputs, outputs);

            Inputs = inputs;
            Outputs = outputs;
            Fee = fee ?? throw RequestValidationException.ShouldBeNotNull(nameof(fee));
            Expiration = expiration;
        }
    }
}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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

        public EstimateSendingTransactionRequest(
            ICollection<Input> inputs,
            ICollection<Output> outputs,
            FeeOptions fee)
        {
            SendingTransactionInputsOutputsValidator.Validate(inputs, outputs);

            Inputs = inputs;
            Outputs = outputs;
            Fee = fee ?? throw new ArgumentNullException(nameof(fee));
        }
    }
}

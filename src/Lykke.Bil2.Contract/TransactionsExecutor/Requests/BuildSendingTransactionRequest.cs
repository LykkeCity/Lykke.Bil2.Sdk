using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
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
        /// Transaction transfers.
        /// </summary>
        [JsonProperty("transfers")]
        public IReadOnlyCollection<Transfer> Transfers { get; }

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

        /// <summary>
        /// Endpoint: [POST] /api/transactions/sending/built
        /// </summary>
        public BuildSendingTransactionRequest(
            IReadOnlyCollection<Transfer> transfers, 
            FeeOptions fee, 
            ExpirationOptions expiration = null)
        {
            SendingTransactionTransfersValidator.Validate(transfers);

            Transfers = transfers;
            Fee = fee ?? throw RequestValidationException.ShouldBeNotNull(nameof(fee));
            Expiration = expiration;
        }
    }
}

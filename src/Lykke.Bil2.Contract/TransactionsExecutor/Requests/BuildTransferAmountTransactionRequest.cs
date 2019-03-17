using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/built/transfers/amount
    /// </summary>
    [PublicAPI]
    public class BuildTransferAmountTransactionRequest
    {
        /// <summary>
        /// Transaction transfers.
        /// </summary>
        [JsonProperty("transfers")]
        public IReadOnlyCollection<Transfer> Transfers { get; }

        /// <summary>
        /// Fees amount in particular asset to spend for the given transaction.
        /// </summary>
        [JsonProperty("fees")]
        public IReadOnlyCollection<Fee> Fees { get; }

        /// <summary>
        /// Optional.
        /// Transaction expiration options. Used if blockchain
        /// supports transaction expiration. If blockchain supports
        /// transaction expiration and the value is omitted,
        /// it should be interpreted as infinite expiration.
        /// If several expiration options are specified at once,
        /// and blockchain supports both of them, then transaction
        /// should be expired when earliest condition is met.
        /// </summary>
        [CanBeNull]
        [JsonProperty("expiration")]
        public ExpirationOptions Expiration { get; }

        /// <summary>
        /// Endpoint: [POST] /api/transactions/built/transfers/amount
        /// </summary>
        /// <param name="transfers">Transaction transfers.</param>
        /// <param name="fees">Fees amount in particular asset to spend for the given transaction.</param>
        /// <param name="expiration">
        /// Optional.
        /// Transaction expiration options. Used if blockchain
        /// supports transaction expiration. If blockchain supports
        /// transaction expiration and the value is omitted,
        /// it should be interpreted as infinite expiration.
        /// If several expiration options are specified at once,
        /// and blockchain supports both of them, then transaction
        /// should be expired when earliest condition is met.
        /// </param>
        public BuildTransferAmountTransactionRequest(
            IReadOnlyCollection<Transfer> transfers, 
            IReadOnlyCollection<Fee> fees,
            ExpirationOptions expiration = null)
        {
            if (fees == null)
                throw RequestValidationException.ShouldBeNotNull(nameof(fees));

            TransactionTransfersValidator.Validate(transfers);
            FeesValidator.ValidateFeesInRequest(fees);
            
            Transfers = transfers;
            Fees = fees;
            Expiration = expiration;
        }
    }
}

using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Numerics;
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
        /// Fee amount in particular asset to spend for the given transaction.
        /// </summary>
        [JsonProperty("fee")]
        public IReadOnlyDictionary<AssetId, UMoney> Fee { get; }

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
        /// <param name="fee">Fee amount in particular asset to spend for the given transaction.</param>
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
            IReadOnlyDictionary<AssetId, UMoney> fee,
            ExpirationOptions expiration = null)
        {
            TransactionTransfersValidator.Validate(transfers);

            Transfers = transfers;
            Fee = fee ?? throw RequestValidationException.ShouldBeNotNull(nameof(fee));
            Expiration = expiration;
        }
    }
}

using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.Common.JsonConverters;
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
        /// Endpoint: [POST] /api/transactions/built/transfers/amount
        /// </summary>
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

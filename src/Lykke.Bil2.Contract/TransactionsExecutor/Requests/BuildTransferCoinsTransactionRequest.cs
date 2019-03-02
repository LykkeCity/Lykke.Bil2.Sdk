using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/built/transfers/coins
    /// </summary>
    [PublicAPI]
    public class BuildTransferCoinsTransactionRequest
    {
        /// <summary>
        /// The coins which should be spend within the transaction.
        /// </summary>
        [JsonProperty("coinsToSpend")]
        public IReadOnlyCollection<CoinToSpend> CoinsToSpend { get; }

        /// <summary>
        /// The coins which should be received within the transaction.
        /// </summary>
        [JsonProperty("coinsToReceive")]
        public IReadOnlyCollection<CoinToReceive> CoinsToReceive { get; }

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
        /// Endpoint: [POST] /api/transactions/built/transfers/coins
        /// </summary>
        public BuildTransferCoinsTransactionRequest(
            IReadOnlyCollection<CoinToSpend> coinsToSpend, 
            IReadOnlyCollection<CoinToReceive> coinsToReceive,
            FeeOptions fee, 
            ExpirationOptions expiration = null)
        {
            TransactionCoinsValidator.Validate(coinsToSpend, coinsToReceive);

            CoinsToSpend = coinsToSpend;
            CoinsToReceive = coinsToReceive;
            Fee = fee ?? throw RequestValidationException.ShouldBeNotNull(nameof(fee));
            Expiration = expiration;
        }
    }
}

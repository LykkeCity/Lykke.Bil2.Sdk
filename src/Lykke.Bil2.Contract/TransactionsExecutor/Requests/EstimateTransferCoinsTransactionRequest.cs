using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    /// <summary>
    /// Endpoint: [POST] /api/transactions/estimated/transfers/coins
    /// </summary>
    [PublicAPI]
    public class EstimateTransferCoinsTransactionRequest
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
        /// Endpoint: [POST] /api/transactions/estimated/transfers/coins
        /// </summary>
        public EstimateTransferCoinsTransactionRequest(
            IReadOnlyCollection<CoinToSpend> coinsToSpend, 
            IReadOnlyCollection<CoinToReceive> coinsToReceive)
        {
            TransactionCoinsValidator.Validate(coinsToSpend, coinsToReceive);

            CoinsToSpend = coinsToSpend;
            CoinsToReceive = coinsToReceive;
        }
    }
}

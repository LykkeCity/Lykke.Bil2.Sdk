using System;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses
{
    public class BlockchainInfo
    {
        /// <summary>
        /// Number of the latest available block in the blockchain according to the integration.
        /// </summary>
        [JsonProperty("latestBlockNumber")]
        public long LatestBlockNumber { get; }

        /// <summary>
        /// Moment of the latest available block in the blockchain according to the integration.
        /// </summary>
        [JsonProperty("latestBlockMoment")]
        public DateTime LatestBlockMoment { get; }

        public BlockchainInfo(long latestBlockNumber, DateTime latestBlockMoment)
        {
            if(latestBlockNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(latestBlockNumber), latestBlockNumber, "Should be positive number");

            LatestBlockNumber = latestBlockNumber;
            LatestBlockMoment = latestBlockMoment;
        }
    }

}

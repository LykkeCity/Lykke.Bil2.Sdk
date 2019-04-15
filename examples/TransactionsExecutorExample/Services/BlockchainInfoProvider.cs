using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace TransactionsExecutorExample.Services
{
    public class BlockchainInfoProvider : IBlockchainInfoProvider
    {
        private readonly DateTime _startTime;

        public BlockchainInfoProvider()
        {
            _startTime = new DateTime(2019, 1, 1);
        }

        public Task<BlockchainInfo> GetInfoAsync()
        {
            var now = DateTime.UtcNow;
            var blockTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second / 30 * 30);
            var blockNumber = (int)Math.Round((blockTime - _startTime).TotalSeconds / 30);

            return Task.FromResult(new BlockchainInfo(blockNumber, blockTime));
        }
    }
}

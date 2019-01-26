using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Models;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services;

namespace TransactionsExecutorExample.Services
{
    public class IntegrationInfoService : IIntegrationInfoService
    {
        private readonly DateTime _startTime;

        public IntegrationInfoService()
        {
            _startTime = new DateTime(2019, 1, 1);
        }

        public Task<IntegrationInfo> GetInfoAsync()
        {
            var now = DateTime.UtcNow;
            var blockTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second / 30 * 30);
            var blockNumber = (int)Math.Round((blockTime - _startTime).TotalSeconds / 30);

            return Task.FromResult(new IntegrationInfo
            (
                new BlockchainInfo(blockNumber, blockTime),
                new Dictionary<string, DependencyInfo>
                {
                    {"node", new DependencyInfo(new Version(1, 2, 3), new Version(1, 4, 2))}
                }
            ));
        }
    }
}

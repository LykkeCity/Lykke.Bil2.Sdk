using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services;
using TransactionsExecutorExample.Settings;

namespace TransactionsExecutorExample.Services
{
    public class IntegrationInfoService : IIntegrationInfoService
    {
        private readonly AppSettings _settings;
        private readonly DateTime _startTime;

        public IntegrationInfoService(AppSettings appSettings)
        {
            _settings = appSettings;
            _startTime = new DateTime(2019, 1, 1);
        }

        public Task<IntegrationInfoResponse> GetInfoAsync()
        {
            var now = DateTime.UtcNow;
            var blockTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second / 30 * 30);
            var blockNumber = (int)Math.Round((blockTime - _startTime).TotalSeconds / 30);

            return Task.FromResult(new IntegrationInfoResponse
            (
                new Dictionary<string, string>
                {
                    {nameof(AppSettings.HealthMonitoringPeriod), _settings.HealthMonitoringPeriod.ToString()},
                    {nameof(DbSettings.AzureDataConnString), _settings.Db.AzureDataConnString}
                },
                new BlockchainInfo(blockNumber, blockTime),
                new Dictionary<string, DependencyInfo>
                {
                    {"node", new DependencyInfo(new Version(1, 2, 3), new Version(1, 4, 2))}
                }
            ));
        }
    }
}

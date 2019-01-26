using System;
using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services;

namespace TransactionsExecutorExample.Services
{
    public class HealthProvider : IHealthProvider
    {
        private readonly Random _random;

        public HealthProvider()
        {
            _random = new Random();
        }

        public Task<string> GetDiseaseAsync()
        {
            var value = _random.Next(0, 100);

            if (value < 30)
            {
                return Task.FromResult("Node is unavailable");
            }

            if (value < 40)
            {
                return Task.FromResult("Node is not synchronized");
            }

            return Task.FromResult<string>(null);
        }
    }
}

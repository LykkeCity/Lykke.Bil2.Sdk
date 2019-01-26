using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Sdk;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services
{
    internal class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly IHealthMonitor _healthMonitor;
        
        public StartupManager(
            ILogFactory logFactory,
            IHealthMonitor healthMonitor)
        {
            _healthMonitor = healthMonitor;
            _log = logFactory.CreateLog(this);
        }

        public Task StartAsync()
        {
            _log.Info("Starting health monitor...");

            _healthMonitor.Start();

            return Task.CompletedTask;
        }
    }
}
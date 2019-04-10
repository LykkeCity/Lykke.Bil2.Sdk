using System.Threading.Tasks;
using Common.Log;
using Lykke.Bil2.Sdk.Services;
using Lykke.Common.Log;
using Lykke.Sdk;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    internal class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly IHealthMonitor _healthMonitor;
        private readonly IHealthProvider _healthProvider;
        private readonly IBlockchainInfoProvider _blockchainInfoProvider;
        private readonly IDependenciesInfoProvider _dependenciesInfoProvider;
        private readonly ISettingsRenderer _settingsRenderer;

        public StartupManager(
            ILogFactory logFactory,
            IHealthMonitor healthMonitor,
            IHealthProvider healthProvider,
            IBlockchainInfoProvider blockchainInfoProvider,
            IDependenciesInfoProvider dependenciesInfoProvider,
            ISettingsRenderer settingsRenderer)
        {
            _log = logFactory.CreateLog(this);
            _healthMonitor = healthMonitor;
            _healthProvider = healthProvider;
            _blockchainInfoProvider = blockchainInfoProvider;
            _dependenciesInfoProvider = dependenciesInfoProvider;
            _settingsRenderer = settingsRenderer;
        }

        public async Task StartAsync()
        {
            _log.Info("Settings", _settingsRenderer.RenderSettings());

            _log.Info("Starting health monitor...");

            _healthMonitor.Start();

            _log.Info("Getting integration health...");

            var disease = await _healthProvider.GetDiseaseAsync();

            if (disease == null)
            {
                _log.Info("Integration is healthy");
            }
            else
            {
                _log.Warning($"Integration is unhealthy: {disease}");
            }

            _log.Info("Getting blockchain info...");

            var blockchainInfo = await _blockchainInfoProvider.GetInfoAsync();

            _log.Info("Blockchain info", blockchainInfo);

            _log.Info("Getting dependencies info...");

            var dependenciesInfo = await _dependenciesInfoProvider.GetInfoAsync();

            _log.Info("Dependencies info", dependenciesInfo);
        }
    }
}

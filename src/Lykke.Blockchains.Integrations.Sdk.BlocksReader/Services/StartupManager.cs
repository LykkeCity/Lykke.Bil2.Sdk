using System.Threading.Tasks;
using Common.Log;
using Lykke.Blockchains.Integrations.Sdk.Services;
using Lykke.Common.Log;
using Lykke.Sdk;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader.Services
{
    internal class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly IRabbitMqConfigurator _rabbitMqConfigurator;
        private readonly IIrreversibleBlockMonitor _irreversibleBlockMonitor;
        private readonly ISettingsRenderer _settingsRenderer;

        public StartupManager(
            ILogFactory logFactory,
            IRabbitMqConfigurator rabbitMqConfigurator,
            ISettingsRenderer settingsRenderer,
            IIrreversibleBlockMonitor irreversibleBlockMonitor = null)
        {
            _log = logFactory.CreateLog(this);
            _rabbitMqConfigurator = rabbitMqConfigurator;
            _irreversibleBlockMonitor = irreversibleBlockMonitor;
            _settingsRenderer = settingsRenderer;
        }

        public Task StartAsync()
        {
            _log.Info("Settings", _settingsRenderer.RenderSettings());

            _log.Info("Configuring messaging...");

            _rabbitMqConfigurator.Configure();

            if (_irreversibleBlockMonitor != null)
            {
                _log.Info("Starting last irreversible block monitor...");

                _irreversibleBlockMonitor.Start();
            }

            return Task.CompletedTask;
        }
    }
}

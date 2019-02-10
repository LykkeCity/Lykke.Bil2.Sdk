using System;
using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Sdk.Services;
using Lykke.Common.Log;
using Lykke.Sdk;

namespace Lykke.Blockchains.Integrations.Sdk.SignService.Services
{
    internal class StartupManager : IStartupManager
    {
        private readonly ISettingsRenderer _settingsRenderer;
        
        public StartupManager(
            ISettingsRenderer settingsRenderer)
        {
            _settingsRenderer = settingsRenderer;
        }

        public Task StartAsync()
        {
            Console.WriteLine("Settings:");
            Console.WriteLine(LogContextConversion.ConvertToString(_settingsRenderer.RenderSettings()));

            return Task.CompletedTask;
        }
    }
}

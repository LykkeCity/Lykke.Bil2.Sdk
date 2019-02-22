using System;
using System.Threading.Tasks;
using Lykke.Bil2.Sdk.Services;
using Lykke.Common.Log;
using Lykke.Sdk;

namespace Lykke.Bil2.Sdk.SignService.Services
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

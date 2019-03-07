using System;
using System.IO;
using Lykke.Bil2.Client.BlocksReader.Tests.Configuration;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Tests
{
    public class SettingsMock
    {
        private readonly string _pathToSettings;

        public SettingsMock(string pathToSettings)
        {
            _pathToSettings = pathToSettings;
            Environment.SetEnvironmentVariable("DisableAutoRegistrationInMonitoring", "true");
            Environment.SetEnvironmentVariable("SettingsUrl", _pathToSettings);
        }

        public void PrepareSettings(AppSettings settings)
        {
            string serializedSettings = Newtonsoft.Json.JsonConvert.SerializeObject(settings);

            try
            {
                File.Delete(_pathToSettings);
            }
            catch
            {
            }

            File.AppendAllText(_pathToSettings, serializedSettings);
        }
    }
}
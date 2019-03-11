using System;
using System.IO;

namespace Lykke.Bil2.BaseTests
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

        public void PrepareSettings<TAppSettings>(TAppSettings settings)
        {
            var serializedSettings = Newtonsoft.Json.JsonConvert.SerializeObject(settings);

            try
            {
                File.Delete(_pathToSettings);
            }
            catch
            {
                // ignored
            }

            File.AppendAllText(_pathToSettings, serializedSettings);
        }
    }
}

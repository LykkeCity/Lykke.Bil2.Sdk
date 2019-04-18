using Lykke.Bil2.Sdk.BlocksReader.Settings;
using Lykke.Bil2.Sdk.Services;

namespace BlocksReaderExample.Settings
{
    public class AppSettings : BaseBlocksReaderSettings<DbSettings, RabbitMqSettings>
    {
        public string NodeUrl { get; set; }

        public string NodeUser { get; set; }

        [SecureSettings]
        public string NodePassword { get; set; }
    }
}

using Lykke.Blockchains.Integrations.Sdk.BlocksReader.Settings;
using Lykke.Blockchains.Integrations.Sdk.Services;

namespace BlocksReaderExample.Settings
{
    public class AppSettings : BaseBlocksReaderSettings<DbSettings>
    {
        public string NodeUrl { get; set; }

        public string NodeUser { get; set; }

        [SecureSettings]
        public string NodePassword { get; set; }
    }
}

using Lykke.Bil2.Sdk.BlocksReader.Settings;
using Lykke.Bil2.Sdk.Services;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Configuration
{
    public class AppSettings : BaseBlocksReaderSettings<DbSettings>
    {
        public string NodeUrl { get; set; }

        public string NodeUser { get; set; }

        [SecureSettings]
        public string NodePassword { get; set; }
    }
}

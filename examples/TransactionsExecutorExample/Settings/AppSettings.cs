using Lykke.Blockchains.Integrations.Sdk.Services;
using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Settings;

namespace TransactionsExecutorExample.Settings
{
    public class AppSettings : BaseTransactionsExecutorSettings<DbSettings>
    {
        public string NodeUrl { get; set; }

        public string NodeUser { get; set; }

        [SecureSettings]
        public string NodePassword { get; set; }
    }
}

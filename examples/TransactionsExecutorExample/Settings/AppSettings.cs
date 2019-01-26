using Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Settings;

namespace TransactionsExecutorExample.Settings
{
    public class AppSettings : BaseTransactionsExecutorSettings<DbSettings>
    {
        public string NodeUrl { get; set; }
    }
}

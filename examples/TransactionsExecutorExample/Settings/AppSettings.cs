using Lykke.Bil2.Sdk.Services;
using Lykke.Bil2.Sdk.TransactionsExecutor.Settings;

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

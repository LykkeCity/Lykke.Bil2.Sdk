using Lykke.Bil2.Sdk.Services;
using Lykke.Bil2.Sdk.TransactionsExecutor.Settings;

namespace Lykke.Bil2.Client.TransactionExecutor.Tests.Configuration
{
    public class AppSettings : BaseTransactionsExecutorSettings<DbSettings>
    {
        public string NodeUrl { get; set; }

        public string NodeUser { get; set; }

        [SecureSettings]
        public string NodePassword { get; set; }
    }
}

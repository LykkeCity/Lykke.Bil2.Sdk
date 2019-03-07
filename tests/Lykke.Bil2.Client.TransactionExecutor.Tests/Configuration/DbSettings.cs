using Lykke.Bil2.Sdk.TransactionsExecutor.Settings;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Bil2.Client.TransactionExecutor.Tests.Configuration
{
    public class DbSettings : BaseTransactionsExecutorDbSettings
    {
        [Optional]
        public new string AzureDataConnString { get; set; }

        [Optional]
        public new string LogsConnString { get; set; }
    }
}

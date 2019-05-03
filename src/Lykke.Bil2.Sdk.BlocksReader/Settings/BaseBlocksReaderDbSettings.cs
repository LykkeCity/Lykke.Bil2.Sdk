using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Bil2.Sdk.BlocksReader.Settings
{
    /// <summary>
    /// Base database settings of a blocks reader application.
    /// </summary>
    [PublicAPI]
    public class BaseBlocksReaderDbSettings
    {
        [AzureTableCheck]
        public string AzureDataConnString { get; set; }

        [AzureTableCheck]
        public string LogsConnString { get; set; }

        public int MaxTransactionsSavingParallelism { get; set; }
    }
}

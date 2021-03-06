﻿using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Settings
{
    /// <summary>
    /// Base database settings of a transactions executor application.
    /// </summary>
    [PublicAPI]
    public class BaseTransactionsExecutorDbSettings
    {
        [AzureTableCheck]
        public string AzureDataConnString { get; set; }

        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}

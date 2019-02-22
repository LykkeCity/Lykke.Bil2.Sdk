using JetBrains.Annotations;

namespace Lykke.Bil2.Sdk.Repositories
{
    /// <summary>
    /// Tools for the raw transactions repository
    /// </summary>
    [PublicAPI]
    public static class RawTransactionRepositoryTools
    {
        public static string GetContainerName(string integrationName)
        {
            return $"raw-transactions-{integrationName}";
        }

        public static string GetBlobName(string transactionHash)
        {
            return transactionHash;
        }
    }
}

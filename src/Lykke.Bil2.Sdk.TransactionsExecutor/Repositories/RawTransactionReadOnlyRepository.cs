using System.IO;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Blob;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Sdk.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Repositories
{
    internal class RawTransactionReadOnlyRepository : IRawTransactionReadOnlyRepository
    {
        private readonly IBlobStorage _blob;
        private readonly string _containerName;

        public static IRawTransactionReadOnlyRepository Create(
            string integrationName,
            IReloadingManager<string> connectionString)
        {
            var blob = AzureBlobStorage.Create(connectionString);

            return new RawTransactionReadOnlyRepository(integrationName, blob);
        }

        private RawTransactionReadOnlyRepository(string integrationName, IBlobStorage blob)
        {
            _containerName = RawTransactionRepositoryTools.GetContainerName(integrationName);
            _blob = blob;
        }

        public async Task<Base58String> GetOrDefaultAsync(string transactionHash)
        {
            var containerName = GetContainerName();
            var blobName = RawTransactionRepositoryTools.GetBlobName(transactionHash);

            if (!await _blob.HasBlobAsync(containerName, blobName))
            {
                return null;
            }
            
            using (var stream = await _blob.GetAsync(containerName, blobName))
            using (var textReader = new StreamReader(stream))
            {
                stream.Position = 0;

                return new Base58String(await textReader.ReadToEndAsync());
            }
        }

        private string GetContainerName()
        {
            return _containerName;
        }
    }
}

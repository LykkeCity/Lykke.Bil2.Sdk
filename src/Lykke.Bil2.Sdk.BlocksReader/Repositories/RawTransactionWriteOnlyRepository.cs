using System.IO;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Blob;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Sdk.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Bil2.Sdk.BlocksReader.Repositories
{
    internal class RawTransactionWriteOnlyRepository : IRawTransactionWriteOnlyRepository
    {
        private readonly IBlobStorage _blob;
        private readonly string _containerName;

        public static IRawTransactionWriteOnlyRepository Create(
            string integrationName,
            IReloadingManager<string> connectionString)
        {
            var blob = AzureBlobStorage.Create(connectionString);

            return new RawTransactionWriteOnlyRepository(integrationName, blob);
        }

        private RawTransactionWriteOnlyRepository(string integrationName, IBlobStorage blob)
        {
            _containerName = RawTransactionRepositoryTools.GetContainerName(integrationName);
            _blob = blob;
        }

        public async Task SaveAsync(string transactionHash, Base58String rawTransaction)
        {
            var containerName = GetContainerName();
            var blobName = RawTransactionRepositoryTools.GetBlobName(transactionHash);

            using (var stream = new MemoryStream())
            using (var textWriter = new StreamWriter(stream))
            {
                textWriter.Write(rawTransaction.Value);

                await textWriter.FlushAsync();
                await stream.FlushAsync();

                stream.Position = 0;

                await _blob.SaveBlobAsync(containerName, blobName, stream);
            }
        }

        private string GetContainerName()
        {
            return _containerName;
        }
    }
}

using System.IO;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Blob;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Sdk.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Repositories
{
    internal class RawObjectReadOnlyRepository : IRawObjectReadOnlyRepository
    {
        private readonly string _integrationName;
        private readonly IBlobStorage _blob;

        public static IRawObjectReadOnlyRepository Create(
            string integrationName,
            IReloadingManager<string> connectionString)
        {
            var blob = AzureBlobStorage.Create(connectionString);

            return new RawObjectReadOnlyRepository(integrationName, blob);
        }

        private RawObjectReadOnlyRepository(string integrationName, IBlobStorage blob)
        {
            _integrationName = integrationName;
            _blob = blob;
        }

        public async Task<Base58String> GetOrDefaultAsync(RawObjectType objectType, string objectId)
        {
            var containerName = RawObjectRepositoryTools.GetContainerName(_integrationName, objectType);
            var blobName = RawObjectRepositoryTools.GetBlobName(objectId);

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
    }
}

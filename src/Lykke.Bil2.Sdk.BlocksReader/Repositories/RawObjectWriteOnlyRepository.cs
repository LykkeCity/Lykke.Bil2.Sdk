using System.IO;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Blob;
using Lykke.Bil2.Sdk.Repositories;
using Lykke.Bil2.SharedDomain;
using Lykke.SettingsReader;

namespace Lykke.Bil2.Sdk.BlocksReader.Repositories
{
    internal class RawObjectWriteOnlyRepository : IRawObjectWriteOnlyRepository
    {
        private readonly string _integrationName;
        private readonly IBlobStorage _blob;

        public static IRawObjectWriteOnlyRepository Create(
            string integrationName,
            IReloadingManager<string> connectionString)
        {
            var blob = AzureBlobStorage.Create(connectionString);

            return new RawObjectWriteOnlyRepository(integrationName, blob);
        }

        private RawObjectWriteOnlyRepository(string integrationName, IBlobStorage blob)
        {
            _integrationName = integrationName;
            _blob = blob;
        }

        public async Task SaveAsync(RawObjectType objectType, string objectId, Base58String rawObject)
        {
            var containerName = RawObjectRepositoryTools.GetContainerName(_integrationName, objectType);
            var blobName = RawObjectRepositoryTools.GetBlobName(objectId);

            using (var stream = new MemoryStream())
            using (var textWriter = new StreamWriter(stream))
            {
                textWriter.Write(rawObject.Value);

                await textWriter.FlushAsync();
                await stream.FlushAsync();

                stream.Position = 0;

                await _blob.SaveBlobAsync(containerName, blobName, stream);
            }
        }
    }
}

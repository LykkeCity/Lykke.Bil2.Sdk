using System.Threading.Tasks;
using Lykke.Bil2.Sdk.Repositories;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.BlocksReader.Repositories
{
    internal interface IRawObjectWriteOnlyRepository
    {
        Task SaveAsync(RawObjectType objectType, string objectId, Base58String rawObject);
    }
}

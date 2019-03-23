using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Sdk.Repositories;

namespace Lykke.Bil2.Sdk.BlocksReader.Repositories
{
    internal interface IRawObjectWriteOnlyRepository
    {
        Task SaveAsync(RawObjectType objectType, string objectId, Base58String rawObject);
    }
}

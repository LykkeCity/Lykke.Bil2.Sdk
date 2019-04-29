using System.Threading.Tasks;
using Lykke.Bil2.Sdk.Repositories;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Repositories
{
    internal interface IRawObjectReadOnlyRepository
    {
        Task<Base64String> GetOrDefaultAsync(RawObjectType objectType, string objectId);
    }
}

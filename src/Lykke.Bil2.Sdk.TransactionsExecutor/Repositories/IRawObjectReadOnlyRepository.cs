using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Sdk.Repositories;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Repositories
{
    internal interface IRawObjectReadOnlyRepository
    {
        Task<Base58String> GetOrDefaultAsync(RawObjectType objectType, string objectId);
    }
}

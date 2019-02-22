using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Repositories
{
    internal interface IRawTransactionReadOnlyRepository
    {
        Task<Base58String> GetOrDefaultAsync(string transactionHash);
    }
}

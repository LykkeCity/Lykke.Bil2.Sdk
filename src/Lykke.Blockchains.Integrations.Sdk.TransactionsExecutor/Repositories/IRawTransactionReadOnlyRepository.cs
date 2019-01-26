using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.Common;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Repositories
{
    internal interface IRawTransactionReadOnlyRepository
    {
        Task<Base58String> GetOrDefaultAsync(string transactionHash);
    }
}

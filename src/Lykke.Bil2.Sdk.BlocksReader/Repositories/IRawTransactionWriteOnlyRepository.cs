using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;

namespace Lykke.Bil2.Sdk.BlocksReader.Repositories
{
    internal interface IRawTransactionWriteOnlyRepository
    {
        Task SaveAsync(string transactionId, Base58String rawTransaction);
    }
}

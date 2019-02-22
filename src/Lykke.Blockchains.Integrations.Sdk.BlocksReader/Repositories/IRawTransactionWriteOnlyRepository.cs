using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.Common;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader.Repositories
{
    internal interface IRawTransactionWriteOnlyRepository
    {
        Task SaveAsync(string transactionHash, Base58String rawTransaction);
    }
}

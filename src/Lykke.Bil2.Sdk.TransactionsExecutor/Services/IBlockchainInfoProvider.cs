using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    /// <summary>
    /// Provides blockchain related information.
    /// </summary>
    public interface IBlockchainInfoProvider
    {
        /// <summary>
        /// Should return blockchain related information.
        /// All required information could be gathered synchronously in the call, no caching required.
        /// </summary>
        Task<BlockchainInfo> GetInfoAsync();
    }
}

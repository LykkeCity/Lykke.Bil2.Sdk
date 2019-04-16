using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    /// <summary>
    /// Provides state of transactions
    /// </summary>
    [PublicAPI]
    public interface ITransactionsStateProvider
    {
        /// <summary>
        /// Should return transaction state by its id.
        /// </summary>
        /// <remarks>
        /// Implementation notes - memory pool should be checked first to exclude transaction loss due to race condition.
        /// </remarks>
        Task<TransactionState> GetStateAsync(string transactionId);
    }
}

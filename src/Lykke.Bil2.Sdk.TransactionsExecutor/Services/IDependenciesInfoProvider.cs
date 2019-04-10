using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    /// <summary>
    /// Provides information about blockchain-specific integration dependencies.
    /// </summary>
    public interface IDependenciesInfoProvider
    {
        /// <summary>
        /// Should return information about blockchain-specific integration dependencies.
        /// All required information could be gathered synchronously in the call, no caching required.
        /// </summary>
        Task<IReadOnlyDictionary<string, DependencyInfo>> GetInfoAsync();
    }
}

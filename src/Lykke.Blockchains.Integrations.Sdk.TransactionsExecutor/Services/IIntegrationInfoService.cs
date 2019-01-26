using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Responses;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services
{
    /// <summary>
    /// Provider of information about running integration
    /// </summary>
    public interface IIntegrationInfoService
    {
        /// <summary>
        /// Should return information about running integration.
        /// All required information could be gathered synchronously in the call.
        /// </summary>
        /// <returns></returns>
        Task<IntegrationInfoResponse> GetInfoAsync();
    }
}

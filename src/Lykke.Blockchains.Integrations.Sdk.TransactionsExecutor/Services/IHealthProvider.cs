using System.Threading.Tasks;

namespace Lykke.Blockchains.Integrations.Sdk.TransactionsExecutor.Services
{
    /// <summary>
    /// Integration health provider
    /// </summary>
    public interface IHealthProvider
    {
        /// <summary>
        /// Should returns description of the integration disease or null.
        /// All required information could be gathered synchronously in the call.
        /// </summary>
        Task<string> GetDiseaseAsync();
    }
}

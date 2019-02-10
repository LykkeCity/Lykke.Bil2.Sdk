using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.BlocksReader.Events;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader.Services
{
    /// <summary>
    /// Provides last irreversible blocks. 
    /// </summary>
    /// <remarks>
    /// Used only if blockchain has irreversible blocks and irreversible blocks can be only pulled from the blockchain.
    /// </remarks>
    public interface IIrreversibleBlockProvider
    {
        /// <summary>
        /// Returns event contained information about last irreversible block
        /// </summary>
        /// <returns></returns>
        Task<LastIrreversibleBlockUpdatedEvent> GetLastAsync();
    }
}

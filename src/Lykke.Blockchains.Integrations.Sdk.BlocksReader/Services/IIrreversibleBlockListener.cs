using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.BlocksReader.Events;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader.Services
{
    /// <summary>
    /// Listens for the irreversible blocks events. 
    /// </summary>
    /// <remarks>
    /// Used only if blockchain has irreversible blocks.
    /// In case if irreversible blocks can be only pulled from the blockchain, this interface will be utilized by the SDK automatically.
    /// In case if irreversible blocks can be pushed from the blockchain, then application should use this class to notify about next
    /// irreversible block being pushed.
    /// </remarks>
    public interface IIrreversibleBlockListener
    {
        /// <summary>
        /// Should be called when the irreversible block number is updated to the next value.
        /// </summary>
        Task HandleNewLastIrreversableBlockAsync(LastIrreversibleBlockUpdatedEvent evt);
    }
}

using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.BlocksReader.Events;

namespace Lykke.Blockchains.Integrations.Client.BlocksReader.Services
{
    /// <summary>
    /// Handles events from the blockchain integration block reader application.
    /// </summary>
    [PublicAPI]
    public interface IBlockEventsHandler
    {
        /// <summary>
        /// Handles <see cref="BlockHeaderReadEvent"/>.
        /// </summary>
        Task Handle(string integrationName, BlockHeaderReadEvent evt);
        
        /// <summary>
        /// Handles <see cref="TransactionExecutedEvent"/>.
        /// </summary>
        Task Handle(string integrationName, TransactionExecutedEvent evt);
        
        /// <summary>
        /// Handles <see cref="TransactionFailedEvent"/>.
        /// </summary>
        Task Handle(string integrationName, TransactionFailedEvent evt);
    }
}

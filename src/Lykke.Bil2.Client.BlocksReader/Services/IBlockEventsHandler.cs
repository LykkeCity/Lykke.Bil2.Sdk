using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.BlocksReader.Events;

namespace Lykke.Bil2.Client.BlocksReader.Services
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
        /// Handles <see cref="BlockNotFoundEvent"/>
        /// </summary>
        Task Handle(string integrationName, BlockNotFoundEvent evt);
        
        /// <summary>
        /// Handles <see cref="TransferAmountTransactionExecutedEvent"/>.
        /// </summary>
        Task Handle(string integrationName, TransferAmountTransactionExecutedEvent evt);

        /// <summary>
        /// Handles <see cref="TransferCoinsTransactionExecutedEvent"/>.
        /// </summary>
        Task Handle(string integrationName, TransferCoinsTransactionExecutedEvent evt);
        
        /// <summary>
        /// Handles <see cref="TransactionFailedEvent"/>.
        /// </summary>
        Task Handle(string integrationName, TransactionFailedEvent evt);

        /// <summary>
        /// Handles <see cref="LastIrreversibleBlockUpdatedEvent"/>
        /// </summary>
        Task Handle(string integrationName, LastIrreversibleBlockUpdatedEvent evt);
    }
}

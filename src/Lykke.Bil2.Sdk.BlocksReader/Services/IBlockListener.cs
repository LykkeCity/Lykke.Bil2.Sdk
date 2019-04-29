using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.BlocksReader.Services
{
    /// <summary>
    /// Listens for the block reading events.
    /// </summary>
    [PublicAPI]
    public interface IBlockListener
    {
        /// <summary>
        /// Should be called when block header is read.
        /// </summary>
        Task HandleHeaderAsync(BlockHeaderReadEvent evt);

        /// <summary>
        /// Should be called when raw block is read.
        /// </summary>
        Task HandleRawBlockAsync(Base64String rawBlock, BlockId blockId);

        /// <summary>
        /// Should be called when requested block is not found.
        /// </summary>
        Task HandleBlockNotFoundAsync(BlockNotFoundEvent evt);

        /// <summary>
        /// "Transfer amount" transactions model.
        /// Should be called when executed transaction is read from the block.
        /// </summary>
        Task HandleExecutedTransactionAsync(Base64String rawTransaction, TransferAmountTransactionExecutedEvent evt);

        /// <summary>
        /// "Transfer coins" transactions model.
        /// Should be called when executed transaction is read from the block.
        /// </summary>
        Task HandleExecutedTransactionAsync(Base64String rawTransaction, TransferCoinsTransactionExecutedEvent evt);

        /// <summary>
        /// Should be called when failed transaction is read from the block.
        /// </summary>
        Task HandleFailedTransactionAsync(Base64String rawTransaction, TransactionFailedEvent evt);
    }
}

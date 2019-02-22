using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Contract.Common;

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
        /// Should be called when executed transaction is read from the block.
        /// </summary>
        Task HandleExecutedTransactionAsync(Base58String rawTransaction, TransactionExecutedEvent evt);

        /// <summary>
        /// Should be called when failed transaction is read from the block.
        /// </summary>
        Task HandleFailedTransactionAsync(Base58String rawTransaction, TransactionFailedEvent evt);
    }
}

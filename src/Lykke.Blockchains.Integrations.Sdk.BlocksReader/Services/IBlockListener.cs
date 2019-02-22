using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.BlocksReader.Events;
using Lykke.Blockchains.Integrations.Contract.Common;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader.Services
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

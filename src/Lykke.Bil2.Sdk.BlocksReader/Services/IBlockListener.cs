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
        IBlockTransactionsListener StartBlockTransactionsHandling(BlockHeaderReadEvent evt);

        /// <summary>
        /// Should be called when raw block is read. If it's applicable for the particular blockchain.
        /// </summary>
        void HandleRawBlock(Base64String rawBlock, BlockId blockId);

        /// <summary>
        /// Should be called when requested block is not found.
        /// </summary>
        void HandleBlockNotFound(BlockNotFoundEvent evt);
    }
}

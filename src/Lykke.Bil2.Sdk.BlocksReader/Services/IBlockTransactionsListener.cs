using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.BlocksReader.Services
{
    /// <summary>
    /// Listens for the block transactions reading events.
    /// </summary>
    [PublicAPI]
    public interface IBlockTransactionsListener
    {
        /// <summary>
        /// "Transfer amount" transactions model.
        /// Should be called when executed transaction is read from the block.
        /// </summary>
        void HandleExecutedTransaction(TransferAmountExecutedTransaction transaction);

        /// <summary>
        /// "Transfer coins" transactions model.
        /// Should be called when executed transaction is read from the block.
        /// </summary>
        void HandleExecutedTransaction(TransferCoinsExecutedTransaction transaction);

        /// <summary>
        /// Should be called when failed transaction is read from the block.
        /// </summary>
        void HandleFailedTransaction(FailedTransaction transaction);

        /// <summary>
        /// Should be called when raw transaction is read. If it's applicable for the particular blockchain.
        /// </summary>
        Task HandleRawTransactionAsync(Base64String rawTransaction, TransactionId transactionId);
    }
}

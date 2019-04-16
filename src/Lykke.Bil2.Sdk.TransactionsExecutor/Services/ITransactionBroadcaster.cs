using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    /// <summary>
    /// Transaction broadcaster
    /// </summary>
    public interface ITransactionBroadcaster
    {
        /// <summary>
        /// Should broadcast the signed transaction to the blockchain.
        /// </summary>
        /// <exception cref="RequestValidationException">
        /// Should be thrown if a transaction can’t be broadcasted with the given parameters.
        /// It should be guaranteed that the transaction is not included and will not be included to the any blockchain block.
        /// </exception>
        /// <exception cref="TransactionBroadcastingException">
        /// Should be thrown if a transaction cannot be broadcasted and
        /// the reason can be mapped to the <see cref="TransactionBroadcastingError"/>.
        /// </exception>
        /// <exception cref="Exception">
        /// Includes any other exception types not listed above.
        /// Should be thrown if there are any other errors.
        /// It’s not guaranteed if transaction was broadcasted to the blockchain or not.
        /// </exception>
        Task BroadcastAsync(BroadcastTransactionRequest request);
    }
}

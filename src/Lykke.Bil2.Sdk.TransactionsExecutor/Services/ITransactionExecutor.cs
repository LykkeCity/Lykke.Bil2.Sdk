using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    /// <summary>
    /// Transaction executor
    /// </summary>
    public interface ITransactionExecutor
    {
        /// <summary>
        /// Should build a not signed transaction. For the blockchains where “sending” and “receiving”
        /// transactions are distinguished, this endpoint builds the “sending” transactions.
        /// </summary>
        /// <exception cref="RequestValidationException">
        /// Should be thrown if a transaction can’t be built with the given parameters and it will be never possible to
        /// build the transaction with exactly the same parameters.
        /// </exception>
        /// <exception cref="SendingTransactionBuildingException">
        /// Should be thrown if a transaction cannot be built and the reason can be mapped to
        /// the <see cref="SendingTransactionBuildingError"/>
        /// </exception>
        /// <exception cref="Exception">
        /// Includes any other exception types not listed above.
        /// Should be thrown if there are any other errors.
        /// Likely a temporary issue with infrastructure or configuration, request should be repeated later.
        /// </exception>
        Task<BuildSendingTransactionResponse> BuildSendingAsync(BuildSendingTransactionRequest request);

        /// <summary>
        /// Optional.
        /// Should build the not signed “receiving” transaction. This endpoint should be implemented
        /// by the blockchains, which distinguishes “sending” and “receiving” transactions.
        /// </summary>
        /// <exception cref="RequestValidationException">
        /// Should be thrown if a transaction can’t be built with the given parameters.
        /// The given “sending” transaction can’t be received and it will be never possible
        /// to receive the given “sending” transaction.
        /// </exception>
        /// <exception cref="OperationNotSupportedException">
        /// Should be thrown if receiving transactions are not supported by the blockchain.
        /// </exception>
        /// <exception cref="Exception">
        /// Includes any other exception types not listed above.
        /// Should be thrown if there are any other errors.
        /// Likely a temporary issue with infrastructure or configuration, request should be repeated later.
        /// </exception>
        Task<BuildReceivingTransactionResponse> BuildReceivingAsync(BuildReceivingTransactionRequest request);
        
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

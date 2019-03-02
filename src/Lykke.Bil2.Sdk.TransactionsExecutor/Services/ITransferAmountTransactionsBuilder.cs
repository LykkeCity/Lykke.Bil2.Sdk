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
    /// Transactions builder for "transfer amount" transactions model.
    /// </summary>
    public interface ITransferAmountTransactionsBuilder
    {
        /// <summary>
        /// "Transfer amount" transactions model.
        /// Should build a not signed transaction which sends funds if integration uses “transfer amount” transactions model.
        /// Integration should either support “transfer coins”  or “transfer amount” transactions model.
        /// </summary>
        /// <exception cref="RequestValidationException">
        /// Should be thrown if a transaction can’t be built with the given parameters and it will be never possible to
        /// build the transaction with exactly the same parameters.
        /// </exception>
        /// <exception cref="TransactionBuildingException">
        /// Should be thrown if a transaction cannot be built and the reason can be mapped to
        /// the <see cref="TransactionBuildingError"/>
        /// </exception>
        /// <exception cref="OperationNotSupportedException">
        /// Should be thrown by implementation if "transfer amount" transactions model is not supported by the blockchain.
        /// </exception>
        /// <exception cref="Exception">
        /// Includes any other exception types not listed above.
        /// Should be thrown if there are any other errors.
        /// Likely a temporary issue with infrastructure or configuration, request should be repeated later.
        /// </exception>
        Task<BuildTransactionResponse> BuildTransferAmountAsync(BuildTransferAmountTransactionRequest request);
    }
}

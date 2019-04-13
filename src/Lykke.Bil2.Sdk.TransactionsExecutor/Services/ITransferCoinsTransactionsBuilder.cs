using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    /// <summary>
    /// Transactions builder for "transfer coins" transactions model.
    /// </summary>
    public interface ITransferCoinsTransactionsBuilder
    {
        /// <summary>
        /// "Transfer coins" transactions model.
        /// Should build a not signed transaction which sends funds if integration uses “transfer coins” transactions model.
        /// Integration should either support “transfer coins”  or “transfer amount” transactions model.
        /// </summary>
        /// <exception cref="RequestValidationException">
        /// Should be thrown if a transaction can’t be built with the given parameters and it will never be possible to
        /// build the transaction with exactly the same parameters.
        /// </exception>
        /// <exception cref="TransactionBuildingException">
        /// Should be thrown if a transaction cannot be built and the reason can be mapped to
        /// the <see cref="TransactionBuildingError"/>
        /// </exception>
        /// <exception cref="OperationNotSupportedException">
        /// Should be thrown by implementation if "transfer coins" transactions model is not supported by the blockchain.
        /// </exception>
        /// <exception cref="Exception">
        /// Includes any other exception types not listed above.
        /// Should be thrown if there are any other errors.
        /// Likely a temporary issue with infrastructure or configuration, request should be repeated later.
        /// </exception>
        Task<BuildTransactionResponse> BuildTransferCoinsAsync(BuildTransferCoinsTransactionRequest request);
    }
}

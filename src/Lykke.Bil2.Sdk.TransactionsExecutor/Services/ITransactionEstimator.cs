using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    /// <summary>
    /// Transaction fee estimator
    /// </summary>
    public interface ITransactionEstimator
    {
        /// <summary>
        /// Should estimate the transaction fee. For the blockchains where “sending” and “receiving”
        /// transactions are distinguished, this endpoint estimates fee for the “sending” transactions.
        /// </summary>
        /// <exception cref="RequestValidationException">
        /// Should be thrown if a transaction can’t be estimated with the given parameters and it will be never possible to
        /// estimate the transaction with exactly the same parameters.
        /// </exception>
        /// <exception cref="Exception">
        /// Includes any other exception types not listed above.
        /// Should be thrown if there are any other errors.
        /// Likely a temporary issue with infrastructure or configuration, request should be repeated later.
        /// </exception>
        Task<EstimateSendingTransactionResponse> EstimateSendingAsync(EstimateSendingTransactionRequest request);
    }
}

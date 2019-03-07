using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.Exceptions;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    internal class NotSupportedTransferAmountTransactionsBuilder : ITransferAmountTransactionsBuilder
    {
        public Task<BuildTransactionResponse> BuildTransferAmountAsync(BuildTransferAmountTransactionRequest request)
        {
            throw new OperationNotSupportedException("Integration does not support \"Transfer amount\" transactions model.");
        }
    }
}

using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.Exceptions;

namespace Lykke.Bil2.Sdk.TransactionsExecutor.Services
{
    internal class NotSupportedTransferCoinsTransactionsBuilder : ITransferCoinsTransactionsBuilder
    {
        public Task<BuildTransactionResponse> BuildTransferCoinsAsync(BuildTransferCoinsTransactionRequest request)
        {
            throw new OperationNotSupportedException("Integration does not support \"Transfer coins\" transactions model.");
        }
    }
}

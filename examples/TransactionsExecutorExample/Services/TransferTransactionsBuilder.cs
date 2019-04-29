using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;
using Lykke.Bil2.SharedDomain.Extensions;
using Newtonsoft.Json;

namespace TransactionsExecutorExample.Services
{
    public class TransferTransactionsBuilder : ITransferAmountTransactionsBuilder
    {
        private readonly Random _random;

        public TransferTransactionsBuilder()
        {
            _random = new Random();
        }

        public Task<BuildTransactionResponse> BuildTransferAmountAsync(BuildTransferAmountTransactionRequest request)
        {
            var entropy = _random.Next(0, 100);

            if (entropy < 10)
            {
                throw new TransactionBuildingException(TransactionBuildingError.RetryLater, "Node is too busy");
            }

            if (request.Transfers.Count > 1)
            {
                throw new RequestValidationException("Only single input is supported", request.Transfers.Count, nameof(request.Transfers.Count));
            }

            var context = JsonConvert.SerializeObject(request).ToBase64();

            return Task.FromResult(new BuildTransactionResponse(context));
        }
    }
}

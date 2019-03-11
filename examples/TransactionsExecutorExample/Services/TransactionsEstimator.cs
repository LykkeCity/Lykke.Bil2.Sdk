using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Numerics.Money;

namespace TransactionsExecutorExample.Services
{
    public class TransactionsEstimator : ITransferAmountTransactionsEstimator
    {
        public Task<EstimateTransactionResponse> EstimateTransferAmountAsync(EstimateTransferAmountTransactionRequest request)
        {
            if (request.Transfers.Count > 1)
            {
                throw new RequestValidationException("Only single transfer is supported", request.Transfers.Count, nameof(request.Transfers.Count));
            }

            var fee = (UMoney) (request.Transfers.Single().Amount * 0.00001M);

            return Task.FromResult(new EstimateTransactionResponse
            (
                new Dictionary<AssetId, UMoney>
                {
                    {
                        request.Transfers.Single().AssetId,
                        UMoney.Round(fee, 6)
                    }
                }
            ));
        }
    }
}

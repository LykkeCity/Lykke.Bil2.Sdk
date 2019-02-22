using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace TransactionsExecutorExample.Services
{
    public class TransactionsEstimator : ITransactionEstimator
    {
        public Task<EstimateSendingTransactionResponse> EstimateSendingAsync(EstimateSendingTransactionRequest request)
        {
            if (request.Transfers.Count > 1)
            {
                throw new RequestValidationException("Only single transfer is supported", request.Transfers.Count, nameof(request.Transfers.Count));
            }

            var fee = request.Transfers.Single().Amount.ToDecimal() * 0.00001M;

            return Task.FromResult(new EstimateSendingTransactionResponse
            (
                new Dictionary<AssetId, CoinsAmount>
                {
                    {
                        request.Transfers.Single().AssetId,
                        CoinsAmount.FromDecimal(fee, accuracy: 6)
                    }
                }
            ));
        }
    }
}

using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Responses;
using Lykke.Bil2.SharedDomain;
using Lykke.Bil2.WebClient.Exceptions;
using Refit;

namespace Lykke.Bil2.Client.TransactionsExecutor.Exceptions
{
    [PublicAPI]
    public class TransactionBroadcastingWebApiException : BadRequestWebApiException
    {
        public TransactionBroadcastingError ErrorCode { get; }
        public string ErrorMessage { get; }


        public TransactionBroadcastingWebApiException(ApiException inner) : 
            base(inner)
        {
            var response = inner.GetContentAsAsync<BlockchainErrorResponse<TransactionBroadcastingError>>()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            ErrorCode = response.Code;
            ErrorMessage = response.Message;
        }
    }
}

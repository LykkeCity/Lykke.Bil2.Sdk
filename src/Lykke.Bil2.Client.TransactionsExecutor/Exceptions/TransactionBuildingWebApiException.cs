using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Responses;
using Lykke.Bil2.SharedDomain;
using Lykke.Bil2.WebClient.Exceptions;
using Refit;

namespace Lykke.Bil2.Client.TransactionsExecutor.Exceptions
{
    [PublicAPI]
    public class TransactionBuildingWebApiException : BadRequestWebApiException
    {
        public TransactionBuildingError ErrorCode { get; }
        public string ErrorMessage { get; }

        public TransactionBuildingWebApiException(ApiException inner) : 
            base(inner)
        {
            var response = inner
                .GetContentAsAsync<BlockchainErrorResponse<TransactionBuildingError>>()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            ErrorCode = response.Code;
            ErrorMessage = response.Message;
        }
    }
}

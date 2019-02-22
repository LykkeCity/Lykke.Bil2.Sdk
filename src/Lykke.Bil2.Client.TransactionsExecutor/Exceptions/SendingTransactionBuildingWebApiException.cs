using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Responses;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.WebClient.Exceptions;
using Refit;

namespace Lykke.Bil2.Client.TransactionsExecutor.Exceptions
{
    [PublicAPI]
    public class SendingTransactionBuildingWebApiException : BadRequestWebApiException
    {
        public SendingTransactionBuildingError ErrorCode { get; }
        public string ErrorMessage { get; }

        public SendingTransactionBuildingWebApiException(ApiException inner) : 
            base(inner)
        {
            var response = inner
                .GetContentAsAsync<BlockchainErrorResponse<SendingTransactionBuildingError>>()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            ErrorCode = response.Code;
            ErrorMessage = response.Message;
        }
    }
}

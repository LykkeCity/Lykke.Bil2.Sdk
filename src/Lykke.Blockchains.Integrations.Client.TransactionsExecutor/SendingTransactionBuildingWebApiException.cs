using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common.Responses;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor;
using Lykke.Blockchains.Integrations.WebClient.Exceptions;
using Refit;

namespace Lykke.Blockchains.Integrations.Client.TransactionsExecutor
{
    [PublicAPI]
    public class SendingTransactionBuildingWebApiException : BadRequestWebApiException
    {
        public SendingTransactionBuildingError ErrorCode { get; }
        public string ErrorMessage { get; }

        public SendingTransactionBuildingWebApiException(ApiException inner) : 
            base(inner)
        {
            var response = inner.GetContentAs<BlockchainErrorResponse<SendingTransactionBuildingError>>();

            ErrorCode = response.Code;
            ErrorMessage = response.Message;
        }
    }
}

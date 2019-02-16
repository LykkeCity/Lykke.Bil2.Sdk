using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common.Responses;
using Lykke.Blockchains.Integrations.Contract.TransactionsExecutor;
using Lykke.Blockchains.Integrations.WebClient.Exceptions;
using Refit;

namespace Lykke.Blockchains.Integrations.Client.TransactionsExecutor
{
    [PublicAPI]
    public class TransactionBroadcastingWebApiException : BadRequestWebApiException
    {
        public TransactionBroadcastingError ErrorCode { get; }
        public string ErrorMessage { get; }


        public TransactionBroadcastingWebApiException(ApiException inner) : 
            base(inner)
        {
            var response = inner.GetContentAs<BlockchainErrorResponse<TransactionBroadcastingError>>();

            ErrorCode = response.Code;
            ErrorMessage = response.Message;
        }
    }
}
using System.Net;
using Lykke.Blockchains.Integrations.WebClient;
using Refit;

namespace Lykke.Blockchains.Integrations.Client.TransactionsExecutor
{
    internal class SendingTransactionBuildingExceptionMapper : IExceptionMapper
    {
        public void ThrowMappedExceptionOrPassThrough(ApiException ex)
        {
            if (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new SendingTransactionBuildingWebApiException(ex);
            }
        }
    }
}

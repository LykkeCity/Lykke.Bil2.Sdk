using System.Net;
using Lykke.Bil2.Client.TransactionsExecutor.Exceptions;
using Lykke.Bil2.WebClient;
using Refit;

namespace Lykke.Bil2.Client.TransactionsExecutor.ExceptionsMappers
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

using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.WebClient.Exceptions
{
    [PublicAPI]
    public class NotFoundWebApiException : WebApiException
    {
        public NotFoundWebApiException(Refit.ApiException inner) :
            base(inner)
        {
        }
    }
}

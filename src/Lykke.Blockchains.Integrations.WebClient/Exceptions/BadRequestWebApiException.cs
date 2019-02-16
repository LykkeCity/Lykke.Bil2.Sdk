using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.WebClient.Exceptions
{
    [PublicAPI]
    public class BadRequestWebApiException : WebApiException
    {
        public BadRequestWebApiException(Refit.ApiException inner) :
            base(inner)
        {
        }
    }
}

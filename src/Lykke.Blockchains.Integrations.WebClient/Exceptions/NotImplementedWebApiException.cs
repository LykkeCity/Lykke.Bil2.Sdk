using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.WebClient.Exceptions
{
    [PublicAPI]
    public class NotImplementedWebApiException : WebApiException
    {
        public NotImplementedWebApiException(Refit.ApiException inner) :
            base(inner)
        {
        }
    }
}

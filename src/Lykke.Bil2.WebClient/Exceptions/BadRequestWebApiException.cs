using JetBrains.Annotations;

namespace Lykke.Bil2.WebClient.Exceptions
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

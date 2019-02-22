using JetBrains.Annotations;

namespace Lykke.Bil2.WebClient.Exceptions
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

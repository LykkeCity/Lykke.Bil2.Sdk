using JetBrains.Annotations;

namespace Lykke.Bil2.WebClient.Exceptions
{
    [PublicAPI]
    public class InternalServerErrorWebApiException : WebApiException
    {
        public InternalServerErrorWebApiException(Refit.ApiException inner) :
            base(inner)
        {
        }
    }
}

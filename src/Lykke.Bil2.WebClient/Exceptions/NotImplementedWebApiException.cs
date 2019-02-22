using JetBrains.Annotations;

namespace Lykke.Bil2.WebClient.Exceptions
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

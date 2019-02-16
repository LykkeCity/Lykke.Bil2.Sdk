namespace Lykke.Blockchains.Integrations.WebClient.Exceptions
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

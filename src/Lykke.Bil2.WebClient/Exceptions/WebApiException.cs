using System;
using JetBrains.Annotations;

namespace Lykke.Bil2.WebClient.Exceptions
{
    [PublicAPI]
    public class WebApiException : Exception
    {
        public WebApiException(Refit.ApiException inner) :
            base($"Web API request failed. Status code: {inner.StatusCode}. Details: {inner.Content}", inner)
        {
        }
    }
}

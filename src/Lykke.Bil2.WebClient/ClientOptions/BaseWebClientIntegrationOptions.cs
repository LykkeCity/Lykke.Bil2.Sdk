using JetBrains.Annotations;

namespace Lykke.Bil2.WebClient.ClientOptions
{
    /// <summary>
    /// Base web client integration options.
    /// </summary>
    [PublicAPI]
    public class BaseWebClientIntegrationOptions
    {
        /// <summary>
        /// Transactions executor absolute URL of the integration.
        /// </summary>
        public string Url { get; set; }
    }
}

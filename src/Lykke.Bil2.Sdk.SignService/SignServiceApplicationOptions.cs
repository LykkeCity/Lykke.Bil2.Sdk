using JetBrains.Annotations;

namespace Lykke.Bil2.Sdk.SignService
{
    /// <summary>
    /// Options of the blockchain integration sign service application.
    /// </summary>
    [PublicAPI]
    public class SignServiceApplicationOptions
    {
        /// <summary>
        /// Required.
        /// Name of the blockchain integration in CamelCase
        /// </summary>
        public string IntegrationName { get; set; }
    }
}

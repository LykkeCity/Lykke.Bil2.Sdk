using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Sdk.SignService
{
    /// <summary>
    /// Options of the blockchain integration sign service application.
    /// </summary>
    [PublicAPI]
    public class SignServiceApplicationOptions
    {
        /// <summary>
        /// Required.
        /// Name of the integration.
        /// </summary>
        public string IntegrationName { get; set; }
    }
}

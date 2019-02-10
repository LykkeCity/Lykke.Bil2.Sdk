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
        /// Name of the blockchain integration. Words should be separated by the space, each word should be started from the capital case.
        /// </summary>
        public string IntegrationName { get; set; }
    }
}

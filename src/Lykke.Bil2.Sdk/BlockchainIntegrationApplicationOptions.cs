using JetBrains.Annotations;

namespace Lykke.Bil2.Sdk
{
    /// <summary>
    /// Options of the blockchain integration application.
    /// </summary>
    [PublicAPI]
    public class BlockchainIntegrationApplicationOptions
    {
        /// <summary>
        /// Required.
        /// Name of the service.
        /// </summary>
        public string ServiceName { get; set; }
    }
}

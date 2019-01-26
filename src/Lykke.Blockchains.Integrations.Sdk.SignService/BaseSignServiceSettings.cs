using JetBrains.Annotations;
using Lykke.Sdk.Settings;

namespace Lykke.Blockchains.Integrations.Sdk.SignService
{
    /// <summary>
    /// Base settings for a sign service application.
    /// </summary>
    [PublicAPI]
    public class BaseSignServiceSettings : BaseAppSettings
    {
        public string EncryptionPrivateKey { get; set; }
    }
}

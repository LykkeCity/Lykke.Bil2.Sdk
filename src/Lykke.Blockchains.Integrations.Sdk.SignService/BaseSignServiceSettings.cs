using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Sdk.Services;
using Lykke.Sdk.Settings;

namespace Lykke.Blockchains.Integrations.Sdk.SignService
{
    /// <summary>
    /// Base settings for a sign service application.
    /// </summary>
    [PublicAPI]
    public class BaseSignServiceSettings : BaseAppSettings
    {
        [SecureSettings]
        public string EncryptionPrivateKey { get; set; }
    }
}

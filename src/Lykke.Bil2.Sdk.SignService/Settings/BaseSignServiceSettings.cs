using JetBrains.Annotations;
using Lykke.Bil2.Sdk.Services;
using Lykke.Sdk.Settings;

namespace Lykke.Bil2.Sdk.SignService.Settings
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

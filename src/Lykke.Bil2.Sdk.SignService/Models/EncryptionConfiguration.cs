using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.SignService.Models
{
    internal class EncryptionConfiguration
    {
        public Base64String PrivateKey { get; }

        public EncryptionConfiguration(Base64String privateKey)
        {
            PrivateKey = privateKey;
        }
    }
}

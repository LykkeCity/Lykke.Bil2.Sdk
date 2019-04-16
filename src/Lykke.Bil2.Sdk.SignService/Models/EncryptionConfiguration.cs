using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.SignService.Models
{
    internal class EncryptionConfiguration
    {
        public Base58String PrivateKey { get; }

        public EncryptionConfiguration(Base58String privateKey)
        {
            PrivateKey = privateKey;
        }
    }
}

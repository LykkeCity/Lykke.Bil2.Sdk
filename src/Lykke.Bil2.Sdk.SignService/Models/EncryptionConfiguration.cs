using Lykke.Bil2.Contract.Common;

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

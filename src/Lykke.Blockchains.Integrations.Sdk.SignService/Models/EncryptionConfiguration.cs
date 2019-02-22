using Lykke.Blockchains.Integrations.Contract.Common;

namespace Lykke.Blockchains.Integrations.Sdk.SignService.Models
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

using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    [PublicAPI]
    public static class BytesExtensions
    {
        public static Base58String EncodeToBase58(this byte[] value)
        {
            return Base58String.Encode(value);
        }

        public static EncryptedString Encrypt(this byte[] value, Base58String publicKey)
        {
            return EncryptedString.Encrypt(publicKey, value);
        }
    }
}

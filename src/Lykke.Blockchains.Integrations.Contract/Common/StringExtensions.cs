using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    [PublicAPI]
    public static class StringExtensions
    {
        public static Base58String ToBase58(this string value)
        {
            return Base58String.Encode(value);
        }

        public static EncryptedString Encrypt(this string value, Base58String publicKey)
        {
            return EncryptedString.Encrypt(publicKey, value);
        }
    }
}

using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Contract.Common.Extensions
{
    [PublicAPI]
    public static class BytesExtensions
    {
        public static EncryptedString Encrypt(this byte[] value, Base64String publicKey)
        {
            return EncryptedString.Encrypt(publicKey, value);
        }
    }
}

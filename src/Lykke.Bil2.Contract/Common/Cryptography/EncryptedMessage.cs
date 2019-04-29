using System;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.Common.Cryptography
{
    internal class EncryptedMessage
    {
        [JsonProperty("encryptedAesKeys")]
        public Base64String EncryptedAesKeys { get; }

        [JsonProperty("encryptedBody")]
        public Base64String EncryptedBody { get; }

        public EncryptedMessage(Base64String encryptedAesKeys, Base64String encryptedBody)
        {
            EncryptedAesKeys = encryptedAesKeys ?? throw new ArgumentNullException(nameof(encryptedAesKeys));
            EncryptedBody = encryptedBody ?? throw new ArgumentNullException(nameof(encryptedBody));
        }
    }
}

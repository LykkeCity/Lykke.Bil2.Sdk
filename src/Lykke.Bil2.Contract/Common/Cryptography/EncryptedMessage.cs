using System;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.Common.Cryptography
{
    internal class EncryptedMessage
    {
        [JsonProperty("encryptedAesKeys")]
        public Base58String EncryptedAesKeys { get; }

        [JsonProperty("encryptedBody")]
        public Base58String EncryptedBody { get; }

        public EncryptedMessage(Base58String encryptedAesKeys, Base58String encryptedBody)
        {
            EncryptedAesKeys = encryptedAesKeys ?? throw new ArgumentNullException(nameof(encryptedAesKeys));
            EncryptedBody = encryptedBody ?? throw new ArgumentNullException(nameof(encryptedBody));
        }
    }
}

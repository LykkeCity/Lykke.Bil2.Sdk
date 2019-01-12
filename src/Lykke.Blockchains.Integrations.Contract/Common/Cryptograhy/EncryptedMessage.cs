using System;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.Common.Cryptograhy
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

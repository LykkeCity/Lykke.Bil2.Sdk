using System;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.Common.Cryptography
{
    internal class AesKeysEnvelope
    {
        [JsonProperty("key")]
        public Base64String Key { get; }
        
        [JsonProperty("iv")]
        public Base64String Iv { get; }

        public AesKeysEnvelope(Base64String key, Base64String iv)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Iv = iv ?? throw new ArgumentNullException(nameof(iv));
        }
    }
}

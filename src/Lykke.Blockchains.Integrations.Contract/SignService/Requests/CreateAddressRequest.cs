using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.SignService.Requests
{
    /// <summary>
    /// Endpoint: [POST] /api/addresses
    /// </summary>
    [PublicAPI]
    public class CreateAddressRequest
    {
        /// <summary>
        /// Encryption public key which should be used to encrypt
        /// the private key of the address being created before
        /// return it in the encryptedPrivateKey field of the
        /// response. 
        /// </summary>
        [JsonProperty("encryptionPublicKey")]
        public Base58String EncryptionPublicKey { get; }

        public CreateAddressRequest(Base58String encryptionPublicKey)
        {
            if (string.IsNullOrWhiteSpace(encryptionPublicKey?.ToString()))
                throw RequestValidationException.ShouldBeNotEmptyString(nameof(encryptionPublicKey));

            EncryptionPublicKey = encryptionPublicKey;
        }
    }
}

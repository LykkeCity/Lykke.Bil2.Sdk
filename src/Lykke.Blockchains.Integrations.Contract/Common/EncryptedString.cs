using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common.Cryptography;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    /// <summary>
    /// RSA PKCS1/AES encrypted string
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(EncryptedStringJsonConverter))]
    public sealed class EncryptedString
    {
        /// <summary>
        /// Encrypted value
        /// </summary>
        public Base58String EncryptedValue { get; }

        public EncryptedString(Base58String encryptedValue)
        {
            EncryptedValue = encryptedValue ?? throw new ArgumentNullException(nameof(encryptedValue));
        }

        public static EncryptedString Encrypt(Base58String publicKey, string stringToEncrypt)
        {
            if (stringToEncrypt == null)
            {
                return null;
            }

            var bytes = Encoding.UTF8.GetBytes(stringToEncrypt);
            
            return Encrypt(publicKey, bytes);
        }

        public static EncryptedString Encrypt(Base58String publicKey, byte[] bytesToEncrypt)
        {
            if(string.IsNullOrWhiteSpace(publicKey?.ToString()))
                throw new ArgumentException("Should be not empty string", nameof(publicKey));

            if (bytesToEncrypt == null)
            {
                return null;
            }

            using (var aes = new AesManaged())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                var aesKeys = new AesKeysEnvelope(aes.Key.EncodeToBase58(), aes.IV.EncodeToBase58());
                var serializedAesKeys = JsonConvert.SerializeObject(aesKeys).ToBase58();

                var encryptedAesKeys = RsaEncryption.Encrypt(serializedAesKeys.DecodeToBytes(), publicKey.DecodeToBytes());
                
                using (var encryptor = aes.CreateEncryptor())
                using (var encryptedStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);

                    cryptoStream.Flush();
                    cryptoStream.Close();

                    var encryptedBody = encryptedStream.ToArray();
                    var encryptedMessage = new EncryptedMessage(encryptedAesKeys.EncodeToBase58(), encryptedBody.EncodeToBase58());
                    var serializedEncryptedMessage = JsonConvert.SerializeObject(encryptedMessage).ToBase58();

                    return new EncryptedString(serializedEncryptedMessage);
                }
            }
        }

        public string DecryptToString(Base58String privateKey)
        {
            var bytes = DecryptToBytes(privateKey);

            return Encoding.UTF8.GetString(bytes);
        }

        public byte[] DecryptToBytes(Base58String privateKey)
        {
            var encryptedMessage = JsonConvert.DeserializeObject<EncryptedMessage>(EncryptedValue.DecodeToString());
            var decryptedAesKey = Base58String.Encode
            (
                RsaEncryption.Decrypt
                (
                    encryptedMessage.EncryptedAesKeys.DecodeToBytes(),
                    privateKey.DecodeToBytes()
                )
            );
            var aesKeys = JsonConvert.DeserializeObject<AesKeysEnvelope>(decryptedAesKey.DecodeToString());

            using (var aes = new AesManaged())
            {
                using (var encryptor = aes.CreateDecryptor(aesKeys.Key.DecodeToBytes(), aesKeys.Iv.DecodeToBytes()))
                using (var encryptedStream = new MemoryStream(encryptedMessage.EncryptedBody.DecodeToBytes()))
                using (var cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Read))
                using (var plainStream = new MemoryStream())
                {
                    cryptoStream.CopyTo(plainStream);

                    var decryptedBytes = plainStream.ToArray();

                    return decryptedBytes;
                }
            }
        }

        public override string ToString()
        {
            return EncryptedValue.ToString();
        }
    }
}

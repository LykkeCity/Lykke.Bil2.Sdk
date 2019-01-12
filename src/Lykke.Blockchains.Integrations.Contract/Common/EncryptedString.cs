using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.Contract.Common.Cryptograhy;
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
        public Base64String EncryptedValue { get; }

        private EncryptedString(Base64String encryptedValue)
        {
            EncryptedValue = encryptedValue;
        }

        public static EncryptedString Create(Base64String encryptedValue)
        {
            if (encryptedValue == null)
                throw new ArgumentNullException(nameof(encryptedValue));

            return new EncryptedString(encryptedValue);
        }

        public static EncryptedString Encrypt(Base64String publicKey, string stringToEncrypt)
        {
            if (stringToEncrypt == null)
            {
                return null;
            }

            var bytes = Encoding.UTF8.GetBytes(stringToEncrypt);
            
            return Encrypt(publicKey, bytes);
        }

        public static EncryptedString Encrypt(Base64String publicKey, byte[] bytesToEncrypt)
        {
            if(string.IsNullOrWhiteSpace(publicKey))
                throw new ArgumentException("Should be not empty string", nameof(publicKey));

            if (bytesToEncrypt == null)
            {
                return null;
            }

            using (var aes = new AesManaged())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                var aesKeys = new AesKeysEnvelope(aes.Key, aes.IV);
                Base64String serializedAesKeys = JsonConvert.SerializeObject(aesKeys);

                var encryptedAesKeys = RsaEncryption.Encrypt(serializedAesKeys, publicKey);
                
                using (var encryptor = aes.CreateEncryptor())
                using (var encryptedStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);

                    cryptoStream.Flush();
                    cryptoStream.Close();

                    var encryptedBody = encryptedStream.ToArray();
                    var encryptedMessage = new EncryptedMessage(encryptedAesKeys, encryptedBody);

                    Base64String serializedEncryptedMessage = JsonConvert.SerializeObject(encryptedMessage);

                    return new EncryptedString(serializedEncryptedMessage);
                }
            }
        }

        public string DecryptToString(Base64String privateKey)
        {
            var bytes = DecryptToBytes(privateKey);

            return Encoding.UTF8.GetString(bytes);
        }

        public byte[] DecryptToBytes(Base64String privateKey)
        {
            var encryptedMessage = JsonConvert.DeserializeObject<EncryptedMessage>(EncryptedValue);
            Base64String decryptedAesKey = RsaEncryption.Decrypt(encryptedMessage.EncryptedAesKeys, privateKey);
            var aesKeys = JsonConvert.DeserializeObject<AesKeysEnvelope>(decryptedAesKey);

            using (var aes = new AesManaged())
            {               
                using (var encryptor = aes.CreateDecryptor(aesKeys.Key, aesKeys.Iv))
                using (var encryptedStream = new MemoryStream(encryptedMessage.EncryptedBody))
                using (var cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Read))
                using (var plainStream = new MemoryStream())
                {
                    cryptoStream.CopyTo(plainStream);

                    var decryptedBytes = plainStream.ToArray();

                    return decryptedBytes;
                }
            }}

        public override string ToString()
        {
            return EncryptedValue.ToString();
        }
    }
}

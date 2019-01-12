using System;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    /// <summary>
    /// Some binary object or probably json object encoded as base64 string.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(Base64StringJsonConverter))]
    public sealed class Base64String
    {
        private static Regex _formatRegex;

        /// <summary>
        /// Base64 encoded value
        /// </summary>
        public string Base64Value { get; }

        static Base64String()
        {
            _formatRegex = new Regex("^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$", RegexOptions.Compiled);
        }

        private Base64String(string base64Value)
        {
            Base64Value = base64Value;
        }

        public static Base64String Create(string base64Value)
        {
            if (base64Value == null)
                throw new ArgumentNullException(nameof(base64Value));

            if (!_formatRegex.IsMatch(base64Value))
                throw new Base64StringConversionException("String is not valid Base64 string", base64Value);

            return new Base64String(base64Value);
        }

        public static Base64String Encode(string stringValue)
        {
            if (stringValue == null)
            {
                return null;
            }

            var bytes = Encoding.UTF8.GetBytes(stringValue);
            
            return Encode(bytes);
        }

        public static Base64String Encode(byte[] bytesValue)
        {
            if (bytesValue == null)
            {
                return null;
            }
                
            var value = Convert.ToBase64String(bytesValue);

            return new Base64String(value);
        }

        public static implicit operator Base64String(string stringValue)
        {
            return Encode(stringValue);
        }
   
        public static implicit operator Base64String(byte[] bytes)
        {
            return Encode(bytes);
        }

        public static implicit operator string(Base64String value)
        {
            return value?.DecodeToString();
        }

        public static implicit operator byte[](Base64String value)
        {
            return value?.DecodeToBytes();
        }

        public string DecodeToString()
        {
            var bytes = DecodeToBytes();

            return Encoding.UTF8.GetString(bytes);
        }

        public byte[] DecodeToBytes()
        {
            return Convert.FromBase64String(Base64Value);
        }

        public override string ToString()
        {
            return Base64Value;
        }
    }
}

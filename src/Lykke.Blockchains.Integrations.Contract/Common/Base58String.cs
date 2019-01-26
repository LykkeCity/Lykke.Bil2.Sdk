using System;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SimpleBase;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    /// <summary>
    /// Some binary object or probably json object encoded as base58 string.
    /// </summary>
    [PublicAPI]
    [JsonConverter(typeof(Base58StringJsonConverter))]
    public sealed class Base58String
    {
        private static Regex _formatRegex;

        /// <summary>
        /// Base58 encoded value
        /// </summary>
        public string Value { get; }

        static Base58String()
        {
            _formatRegex = new Regex("^[1-9A-HJ-NP-Za-km-z]*$", RegexOptions.Compiled);
        }

        private Base58String(string value)
        {
            Value = value;
        }

        public static Base58String Create(string base58Value)
        {
            if (base58Value == null)
                throw new ArgumentNullException(nameof(base58Value));

            if (!_formatRegex.IsMatch(base58Value))
                throw new Base58StringConversionException("String is not valid Base58 string", base58Value);

            return new Base58String(base58Value);
        }

        public static Base58String Encode(string stringValue)
        {
            if (stringValue == null)
            {
                return null;
            }

            var bytes = Encoding.UTF8.GetBytes(stringValue);
            
            return Encode(bytes);
        }

        public static Base58String Encode(byte[] bytesValue)
        {
            if (bytesValue == null)
            {
                return null;
            }

            var value = Base58.Bitcoin.Encode(bytesValue);

            return new Base58String(value);
        }

        public static implicit operator Base58String(string stringValue)
        {
            return Encode(stringValue);
        }
   
        public static implicit operator Base58String(byte[] bytes)
        {
            return Encode(bytes);
        }

        public static implicit operator string(Base58String value)
        {
            return value?.DecodeToString();
        }

        public static implicit operator byte[](Base58String value)
        {
            return value?.DecodeToBytes().ToArray();
        }

        public string DecodeToString()
        {
            var bytes = DecodeToBytes();

            return Encoding.UTF8.GetString(bytes);
        }

        public Span<byte> DecodeToBytes()
        {
            return Base58.Bitcoin.Decode(Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}

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

        public Base58String(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (!_formatRegex.IsMatch(value))
                throw new Base58StringConversionException("String is not valid Base58 string", value);

            Value = value;
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

        public string DecodeToString()
        {
            var bytes = DecodeToBytes();

            return Encoding.UTF8.GetString(bytes);
        }

        public byte[] DecodeToBytes()
        {
            return Base58.Bitcoin.Decode(Value).ToArray();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}

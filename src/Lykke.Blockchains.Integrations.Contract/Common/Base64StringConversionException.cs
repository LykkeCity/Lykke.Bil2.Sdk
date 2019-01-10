using System;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    /// <summary>
    /// Base64 string conversion exception
    /// </summary>
    public class Base64StringConversionException : Exception
    {
        /// <summary>
        /// Base64 string conversion exception
        /// </summary>
        public Base64StringConversionException(string message, object sourceValue, Exception innerException = null) :
            base(BuildMessage(message, sourceValue), innerException)
        {
        }

        private static string BuildMessage(string message, object sourceValue)
        {
            return $"Value [{sourceValue}] can't be represented as base64 string: {message}";
        }
    }
}
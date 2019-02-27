using System;

namespace Lykke.Bil2.Contract.Common.Exceptions
{
    /// <summary>
    /// Base58 string conversion exception
    /// </summary>
    public class Base58StringConversionException : Exception
    {
        /// <summary>
        /// Base58 string conversion exception
        /// </summary>
        public Base58StringConversionException(string message, object sourceValue, Exception innerException = null) :
            base(BuildMessage(message, sourceValue), innerException)
        {
        }

        private static string BuildMessage(string message, object sourceValue)
        {
            return $"Value [{sourceValue}] can't be represented as base58 string: {message}";
        }
    }
}

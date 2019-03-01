using System;

namespace Lykke.Bil2.Contract.Common.Exceptions
{
    /// <summary>
    /// Coins conversion exception
    /// </summary>
    public class CoinsConversionException : Exception
    {
        /// <summary>
        /// Coins conversion exception
        /// </summary>
        public CoinsConversionException(string message, object sourceValue, Exception innerException = null) :
            base(BuildMessage(message, sourceValue), innerException)
        {
        }

        private static string BuildMessage(string message, object sourceValue)
        {
            return $"Value [{sourceValue}] can't be represented as coins: {message}";
        }
    }
}

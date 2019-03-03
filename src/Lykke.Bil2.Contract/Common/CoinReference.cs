using System;
using Newtonsoft.Json;

namespace Lykke.Bil2.Contract.Common
{
    /// <summary>
    /// Reference to the coin.
    /// </summary>
    public class CoinReference
    {
        /// <summary>
        /// Id of the transaction within which coin was created.
        /// </summary>
        [JsonProperty("transactionId")]
        public string TransactionId { get; }

        /// <summary>
        /// Number of the coin inside the transaction within which coin was created.
        /// </summary>
        [JsonProperty("coinNumber")]
        public int CoinNumber { get; }

        public CoinReference(string transactionId, int coinNumber)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                throw new ArgumentException("Should be not empty string", nameof(transactionId));

            if (coinNumber < 0)
                throw new ArgumentOutOfRangeException(nameof(coinNumber), coinNumber, "Should be zero or positive number");

            TransactionId = transactionId;
            CoinNumber = coinNumber;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Bil2.Contract.Common.Exceptions;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    internal static class ReceivedCoinsValidator
    {
        public static void Validate(IReadOnlyCollection<ReceivedCoin> receivedCoins)
        {
            if(receivedCoins == null)
                throw new ArgumentNullException(nameof(receivedCoins));

            var duplicatedReceivedCoins = receivedCoins
                .GroupBy(x => x.CoinNumber)
                .Select(g => new{
                    CoinNumber = g.Key,
                    Count = g.Count()
                })
                .Where(x => x.Count > 1)
                .ToArray();

            if (duplicatedReceivedCoins.Any())
            {
                var duplicatesMessage = new StringBuilder();

                duplicatesMessage.AppendLine();

                foreach (var x in duplicatedReceivedCoins)
                {
                    duplicatesMessage.AppendLine($"Coin number: {x.CoinNumber} duplicated {x.Count} times");
                }

                throw new RequestValidationException($"Every coin in the received coins should be presented only once. Duplicates are: {duplicatesMessage}");
            }
        }
    }
}

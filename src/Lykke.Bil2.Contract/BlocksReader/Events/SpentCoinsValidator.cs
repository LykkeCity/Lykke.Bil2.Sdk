using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    internal static class SpentCoinsValidator
    {
        public static void Validate(IReadOnlyCollection<CoinReference> spentCoins)
        {
            if(spentCoins == null)
                throw new ArgumentNullException(nameof(spentCoins));

            var duplicatedSpentCoins = spentCoins
                .GroupBy(x => x)
                .Select(g => new{
                    Coin = g,
                    Count = g.Count()
                })
                .Where(x => x.Count > 1)
                .ToArray();

            if (duplicatedSpentCoins.Any())
            {
                var duplicatesMessage = new StringBuilder();

                duplicatesMessage.AppendLine();

                foreach (var x in duplicatedSpentCoins)
                {
                    duplicatesMessage.AppendLine($"Coin: {x.Coin} duplicated {x.Count} times");
                }

                throw new RequestValidationException($"Every coin in the spent coins should be presented only once. Duplicates are: {duplicatesMessage}");
            }
        }
    }
}

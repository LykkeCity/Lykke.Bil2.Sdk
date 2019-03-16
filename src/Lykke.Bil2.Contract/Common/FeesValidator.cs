using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Numerics.Linq;

namespace Lykke.Bil2.Contract.Common
{
    internal static class FeesValidator
    {
        public static void ValidateFees(IReadOnlyCollection<Fee> fees, bool isRequest)
        {
            var duplicatedAssets = fees
                .GroupBy(x => x.Asset)
                .Select(g => new
                {
                    Asset = g.Key,
                    Count = g.Count(),
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .Where(x => x.Count > 1)
                .ToArray();

            if (duplicatedAssets.Any())
            {
                var duplicatesMessage = new StringBuilder();

                duplicatesMessage.AppendLine();

                foreach (var x in duplicatedAssets)
                {
                    duplicatesMessage.AppendLine($"Asset: {x.Asset} duplicated {x.Count} times with total amount {x.TotalAmount}");
                }

                var message = $"Every asset should be presented once in the fees. Duplicates are: {duplicatesMessage}";

                if (isRequest)
                {
                    throw new RequestValidationException(message, nameof(fees));
                }

                throw new ArgumentException(message, nameof(fees));
            }

        }
    }
}

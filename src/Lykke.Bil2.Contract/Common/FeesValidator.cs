using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Numerics.Linq;

namespace Lykke.Bil2.Contract.Common
{
    internal static class FeesValidator
    {
        public static void ValidateFeesInResponse(IReadOnlyCollection<Fee> fees)
        {
            var errorMessage = ValidateFees(fees);

            if (errorMessage != null)
            {
                throw new ArgumentException(errorMessage, nameof(fees));
            }
        }

        public static void ValidateFeesInRequest(IReadOnlyCollection<Fee> fees)
        {
            var errorMessage = ValidateFees(fees);

            if (errorMessage != null)
            {
                throw new RequestValidationException(errorMessage, nameof(fees));
            }
        }

        /// <returns>Error message, or null</returns>
        [Pure]
        private static string ValidateFees(IReadOnlyCollection<Fee> fees)
        {
            if (fees == null)
            {
                return null;
            }

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

                return $"Every asset should be presented once in the fees. Duplicates are: {duplicatesMessage}";
            }

            return null;
        }
    }
}

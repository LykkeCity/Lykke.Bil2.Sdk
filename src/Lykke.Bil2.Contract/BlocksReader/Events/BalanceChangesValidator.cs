using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lykke.Bil2.Contract.BlocksReader.Events
{
    internal static class BalanceChangesValidator
    {
        public static void Validate(IReadOnlyCollection<BalanceChange> balanceChanges)
        {
            if(balanceChanges == null)
                throw new ArgumentNullException(nameof(balanceChanges));

            var duplicatedBalanceChanges = balanceChanges
                .GroupBy(x => new
                {
                    x.Asset,
                    x.Address,
                    x.TransferId
                })
                .Where(x => x.Count() > 1)
                .ToArray();

            if (duplicatedBalanceChanges.Any())
            {
                var duplicatesMessage = new StringBuilder();

                duplicatesMessage.AppendLine();

                foreach (var group in duplicatedBalanceChanges)
                {
                    duplicatesMessage.AppendLine($"Asset: {group.Key.Asset}, address: {group.Key.Address}, transfer: {group.Key.TransferId}");
                }

                throw new ArgumentException(
                    $"Only one balance change per asset, address, and transfer is allowed. Duplicates are: {duplicatesMessage}",
                    nameof(balanceChanges));
            }
        }
    }
}

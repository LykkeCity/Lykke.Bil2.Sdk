using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Bil2.Contract.Common.Exceptions;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    internal static class SendingTransactionTransfersValidator
    {
        public static void Validate(IReadOnlyCollection<Transfer> transfers)
        {
            if(transfers == null || !transfers.Any())
                throw RequestValidationException.ShouldBeNotEmptyCollection(nameof(transfers));

            var duplicatedTransfers = transfers
                .GroupBy(x => new
                {
                    x.AssetId,
                    x.SourceAddress,
                    x.DestinationAddress
                })
                .Where(x => x.Count() > 1)
                .ToArray();

            if (duplicatedTransfers.Any())
            {
                var duplicatesMessage = new StringBuilder();

                duplicatesMessage.AppendLine();

                foreach (var group in duplicatedTransfers)
                {
                    duplicatesMessage.AppendLine($"Asset: {group.Key.AssetId}, source address: {group.Key.SourceAddress}, destination address: {group.Key.DestinationAddress}");
                }

                throw new RequestValidationException(
                    $"Only one transfer per asset, source address, destination address is allowed. Duplicates are: {duplicatesMessage}",
                    nameof(transfers));
            }
        }
    }
}

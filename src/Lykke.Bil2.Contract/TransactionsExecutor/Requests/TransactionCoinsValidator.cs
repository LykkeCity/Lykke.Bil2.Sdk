using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Linq;

namespace Lykke.Bil2.Contract.TransactionsExecutor.Requests
{
    internal static class TransactionCoinsValidator
    {
        public static void Validate(IReadOnlyCollection<CoinToSpend> coinsToSpend, IReadOnlyCollection<CoinToReceive> coinsToReceive)
        {
            if(coinsToSpend == null || !coinsToSpend.Any())
                throw RequestValidationException.ShouldBeNotEmptyCollection(nameof(coinsToSpend));

            if(coinsToReceive == null || !coinsToReceive.Any())
                throw RequestValidationException.ShouldBeNotEmptyCollection(nameof(coinsToReceive));

            var coinsToReceiveNumbers = coinsToReceive.Select(x => x.CoinNumber).OrderBy(x => x).ToArray();

            if (coinsToReceiveNumbers[0] > 0)
            {
                throw new RequestValidationException("Least coins to receive number should be 0", coinsToReceive.Select(x => x.CoinNumber), nameof(coinsToReceive));
            }

            var previousCoinToReceiveNumber = 0;

            foreach (var number in coinsToReceiveNumbers.Skip(1))
            {
                if (number - 1 != previousCoinToReceiveNumber++)
                {
                    throw new RequestValidationException("Coins to receive numbers should be in a row without gaps", coinsToReceive.Select(x => x.CoinNumber), nameof(coinsToReceive));
                }
            }

            var coinsToSpendByAssets = coinsToSpend
                .GroupBy(x => x.AssetId)
                .Select(g => new
                {
                    AssetId = g.Key,
                    Sum = g.Sum(x => x.Value)
                })
                .OrderBy(x => x.AssetId)
                .ToArray();
            var coinsToReceiveByAssets = coinsToReceive
                .GroupBy(x => x.AssetId)
                .Select(g => new
                {
                    AssetId = g.Key,
                    Sum = g.Sum(x => x.Value)
                })
                .OrderBy(x => x.AssetId)
                .ToArray();
            var assetsToSpend = coinsToSpendByAssets.Select(x => x.AssetId.ToString()).ToArray();
            var assetsToReceive = coinsToReceiveByAssets.Select(x => x.AssetId.ToString()).ToArray();

            if (!assetsToSpend.SequenceEqual(assetsToReceive))
                throw new RequestValidationException(
                    "Sets of coins to spend and coins to receive assets should be equal." +
                    $"{Environment.NewLine}Actual coins to spend assets: [{string.Join(", ", assetsToSpend)}]" +
                    $"{Environment.NewLine}Actual coins to receive assets: [{string.Join(", ", assetsToReceive)}]",
                    new[] {nameof(coinsToSpend), nameof(coinsToReceive)});

            var mismatchedAssetSums = coinsToSpendByAssets
                .Join(
                    coinsToReceiveByAssets,
                    x => x.AssetId,
                    x => x.AssetId,
                    (input, output) => new
                    {
                        AssetId = input.AssetId,
                        SumToSpend = input.Sum,
                        SumToReceive = output.Sum
                    })
                .Where(x => x.SumToSpend < x.SumToReceive)
                .ToArray();

            if (mismatchedAssetSums.Any())
            {
                var mismatchesMessage = new StringBuilder();

                mismatchesMessage.AppendLine();

                foreach (var assetSum in mismatchedAssetSums)
                {
                    mismatchesMessage.AppendLine($"Asset: {assetSum.AssetId}, sum to spend: {assetSum.SumToSpend}, sum to receive: {assetSum.SumToReceive}");
                }

                throw new RequestValidationException(
                    $"Sum to spend and to receive of each asset should be equal. Mismatched sum: {mismatchesMessage}",
                    new[] {nameof(coinsToSpend), nameof(coinsToReceive)});
            }
        }
    }
}

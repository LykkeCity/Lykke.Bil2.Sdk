using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Blockchains.Integrations.Contract.Common;

namespace Lykke.Blockchains.Integrations.Contract.TransactionsExecutor.Requests
{
    internal static class SendingTransactionInputsOutputsValidator
    {
        public static void Validate(ICollection<Input> inputs, ICollection<Output> outputs)
        {
            if(inputs == null || !inputs.Any())
                throw RequestValidationException.ShouldBeNotEmptyCollection(nameof(inputs));

            if(outputs == null || !outputs.Any())
                throw RequestValidationException.ShouldBeNotEmptyCollection(nameof(outputs));

            var inputByAssets = inputs
                .GroupBy(x => x.AssetId)
                .Select(g => new
                {
                    AssetId = g.Key,
                    Sum = g.Sum(x => x.Amount)
                })
                .OrderBy(x => x.AssetId)
                .ToArray();
            var outputByAssets = outputs
                .GroupBy(x => x.AssetId)
                .Select(g => new
                {
                    AssetId = g.Key,
                    Sum = g.Sum(x => x.Amount)
                })
                .OrderBy(x => x.AssetId)
                .ToArray();
            var inputAssets = inputByAssets.Select(x => x.AssetId.ToString()).ToArray();
            var outputAssets = outputByAssets.Select(x => x.AssetId.ToString()).ToArray();

            if (!inputAssets.SequenceEqual(outputAssets))
                throw new RequestValidationException(
                    "Assets sets of inputs and outputs should be equal." +
                    $"{Environment.NewLine}Actual input assets: [{string.Join(", ", inputAssets)}]" +
                    $"{Environment.NewLine}Actual output assets: [{string.Join(", ", outputAssets)}]",
                    new[] {nameof(inputs), nameof(outputs)});

            var mismatchedAssetSums = inputByAssets
                .Join(
                    outputByAssets,
                    x => x.AssetId,
                    x => x.AssetId,
                    (input, output) => new
                    {
                        AssetId = input.AssetId,
                        InputSum = input.Sum,
                        OutputSum = output.Sum
                    })
                .Where(x => x.InputSum != x.OutputSum)
                .ToArray();

            if (mismatchedAssetSums.Any())
            {
                var misamtchesMessage = new StringBuilder();

                misamtchesMessage.AppendLine();

                foreach (var assetSum in mismatchedAssetSums)
                {
                    misamtchesMessage.AppendLine($"Asset: {assetSum.AssetId}, input sum: {assetSum.InputSum}, output sum: {assetSum.OutputSum}");
                }

                throw new RequestValidationException(
                    $"Sum of inputs and outputs of each asset should be equal. Mismatched sum: {misamtchesMessage}",
                    new[] {nameof(inputs), nameof(outputs)});
            }
        }
    }
}

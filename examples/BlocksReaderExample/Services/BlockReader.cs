using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using Lykke.Bil2.SharedDomain;
using Lykke.Bil2.SharedDomain.Extensions;
using Lykke.Numerics;

namespace BlocksReaderExample.Services
{
    public class BlockReader : IBlockReader
    {
        public async Task ReadBlockAsync(long blockNumber, IBlockListener listener)
        {
            var blockId = Guid.NewGuid().ToString("N");
            var previousBlockId = Guid.NewGuid().ToString("N");

            listener.HandleRawBlock("raw-block".ToBase64(), blockId);

            var transactionsListener = listener.StartBlockTransactionsHandling
            (
                new BlockHeaderReadEvent
                (
                    blockNumber,
                    blockId,
                    DateTime.UtcNow,
                    100,
                    1,
                    previousBlockId
                )
            );

            var transactionId = new TransactionId(Guid.NewGuid().ToString("N"));

            await transactionsListener.HandleRawTransactionAsync("raw-transaction".ToBase64(), transactionId);
            
            transactionsListener.HandleExecutedTransaction
            (
                new TransferAmountExecutedTransaction
                (
                    transactionNumber: 1,
                    transactionId: transactionId,
                    balanceChanges: new[]
                    {
                        new BalanceChange("1", new Asset("STEEM"), Money.Create(123, 4), "abc")
                    },
                    fees: new[]
                    {
                        new Fee(new Asset("STEEM"), UMoney.Create(0.0001m, 4)),
                    },
                    isIrreversible: true
                )
            );
        }
    }
}

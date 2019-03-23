using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.Sdk.BlocksReader.Services;
using Lykke.Numerics;

namespace BlocksReaderExample.Services
{
    public class BlockReader : IBlockReader
    {
        public async Task ReadBlockAsync(long blockNumber, IBlockListener listener)
        {
            var blockId = Guid.NewGuid().ToString("N");
            var previousBlockId = Guid.NewGuid().ToString("N");

            await listener.HandleHeaderAsync
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

            await listener.HandleRawBlockAsync("raw-block".ToBase58(), blockId);

            await listener.HandleExecutedTransactionAsync
            (
                "raw-transaction".ToBase58(),
                new TransferAmountTransactionExecutedEvent
                (
                    blockId,
                    1,
                    Guid.NewGuid().ToString("N"),
                    new []
                    {
                        new BalanceChange("1", new Asset("STEEM"), Money.Create(123, 4), "abc")
                    },
                    new []
                    {
                        new Fee(new Asset( "STEEM"), UMoney.Create(0.0001m, 4)), 
                    },
                    isIrreversible: true
                )
            );
        }
    }
}

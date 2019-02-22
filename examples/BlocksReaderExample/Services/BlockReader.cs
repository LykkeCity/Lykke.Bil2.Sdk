using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Sdk.BlocksReader.Services;

namespace BlocksReaderExample.Services
{
    public class BlockReader : IBlockReader
    {
        public async Task ReadBlockAsync(long blockNumber, IBlockListener listener)
        {
            var blockHash = Guid.NewGuid().ToString("N");
            var previousBlockHash = Guid.NewGuid().ToString("N");

            await listener.HandleHeaderAsync(new BlockHeaderReadEvent(blockNumber, blockHash, DateTime.UtcNow, 100, 1, previousBlockHash));
            await listener.HandleExecutedTransactionAsync
            (
                "raw-transaction".ToBase58(),
                new TransactionExecutedEvent
                (
                    blockHash,
                    1,
                    Guid.NewGuid().ToString("N"),
                    new List<BalanceChange>
                    {
                        new BalanceChange("1", "1", "STEEM", CoinsChange.FromDecimal(123, 4), "abc")
                    },
                    isIrreversible: true
                )
            );
        }
    }
}

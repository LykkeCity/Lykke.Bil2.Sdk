using System.Threading.Tasks;
using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Bil2.Contract.BlocksReader.Events;

namespace BlocksReaderExampleClient
{
    public class BlockEventsHandler : IBlockEventsHandler
    {
        public Task Handle(string integrationName, BlockHeaderReadEvent evt)
        {
            return Task.CompletedTask;
        }

        public Task Handle(string integrationName, BlockNotFoundEvent evt)
        {
            return Task.CompletedTask;
        }

        public Task Handle(string integrationName, TransferAmountTransactionExecutedEvent evt)
        {
            return Task.CompletedTask;
        }

        public Task Handle(string integrationName, TransferCoinsTransactionExecutedEvent evt)
        {
            return Task.CompletedTask;
        }

        public Task Handle(string integrationName, TransactionFailedEvent evt)
        {
            return Task.CompletedTask;
        }

        public Task Handle(string integrationName, LastIrreversibleBlockUpdatedEvent evt)
        {
            return Task.CompletedTask;
        }
    }
}

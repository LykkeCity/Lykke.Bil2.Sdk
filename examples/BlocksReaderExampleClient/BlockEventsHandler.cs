using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Client.BlocksReader.Services;
using Lykke.Blockchains.Integrations.Contract.BlocksReader.Events;

namespace BlocksReaderExampleClient
{
    public class BlockEventsHandler : IBlockEventsHandler
    {
        public Task Handle(string integrationName, BlockHeaderReadEvent evt)
        {
            return Task.CompletedTask;
        }

        public Task Handle(string integrationName, TransactionExecutedEvent evt)
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

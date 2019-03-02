using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Bil2.Contract.BlocksReader.Events;
using System;
using System.Threading.Tasks;

namespace Lykke.Bil2.Client.BlocksReader.Tests.RabbitMq
{
    public class FakeBlocksEventHandler : IBlockEventsHandler
    {
        public Task Handle(string integrationName, BlockHeaderReadEvent evt)
        {
            throw new NotImplementedException();
        }

        public Task Handle(string integrationName, TransactionExecutedEvent evt)
        {
            throw new NotImplementedException();
        }

        public Task Handle(string integrationName, TransactionFailedEvent evt)
        {
            throw new NotImplementedException();
        }

        public Task Handle(string integrationName, LastIrreversibleBlockUpdatedEvent evt)
        {
            throw new NotImplementedException();
        }
    }
}

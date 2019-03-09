using System.Threading.Tasks;
using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.RabbitMq.Publication;

namespace BlocksReaderExampleClient
{
    public class BlockEventsHandler : IBlockEventsHandler
    {
        public Task HandleAsync(string integrationName, BlockHeaderReadEvent evt, IMessagePublisher publisher)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(string integrationName, BlockNotFoundEvent evt, IMessagePublisher publisher)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(string integrationName, TransferAmountTransactionExecutedEvent evt, IMessagePublisher publisher)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(string integrationName, TransferCoinsTransactionExecutedEvent evt, IMessagePublisher publisher)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(string integrationName, TransactionFailedEvent evt, IMessagePublisher publisher)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(string integrationName, LastIrreversibleBlockUpdatedEvent evt, IMessagePublisher publisher)
        {
            return Task.CompletedTask;
        }
    }
}

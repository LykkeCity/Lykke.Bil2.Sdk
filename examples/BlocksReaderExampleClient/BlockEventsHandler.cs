using System.Threading.Tasks;
using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;

namespace BlocksReaderExampleClient
{
    public class BlockEventsHandler : IBlockEventsHandler
    {
        public Task HandleAsync(string integrationName, BlockHeaderReadEvent evt, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(string integrationName, BlockNotFoundEvent evt, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(string integrationName, TransferAmountTransactionExecutedEvent evt, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(string integrationName, TransferCoinsTransactionExecutedEvent evt, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(string integrationName, TransactionFailedEvent evt, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            return Task.CompletedTask;
        }

        public Task HandleAsync(string integrationName, LastIrreversibleBlockUpdatedEvent evt, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            return Task.CompletedTask;
        }
    }
}

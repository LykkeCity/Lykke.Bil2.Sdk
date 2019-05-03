using System.Threading.Tasks;
using Lykke.Bil2.Client.BlocksReader.Services;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;

namespace BlocksReaderExampleClient
{
    public class BlockEventsHandler : IBlockEventsHandler
    {
        public Task<MessageHandlingResult> HandleAsync(string integrationName, BlockHeaderReadEvent evt, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            return Task.FromResult(MessageHandlingResult.Success());
        }

        public Task<MessageHandlingResult> HandleAsync(string integrationName, BlockNotFoundEvent evt, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            return Task.FromResult(MessageHandlingResult.Success());
        }

        public Task<MessageHandlingResult> HandleAsync(string state, TransferAmountTransactionsBatchEvent message, MessageHeaders headers,
            IMessagePublisher replyPublisher)
        {
            return Task.FromResult(MessageHandlingResult.Success());
        }

        public Task<MessageHandlingResult> HandleAsync(string state, TransferCoinsTransactionsBatchEvent message, MessageHeaders headers,
            IMessagePublisher replyPublisher)
        {
            return Task.FromResult(MessageHandlingResult.Success());
        }

        public Task<MessageHandlingResult> HandleAsync(string integrationName, LastIrreversibleBlockUpdatedEvent evt, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            return Task.FromResult(MessageHandlingResult.Success());
        }
    }
}

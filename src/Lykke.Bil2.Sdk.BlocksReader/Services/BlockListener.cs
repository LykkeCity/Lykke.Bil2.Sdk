using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.Sdk.BlocksReader.Repositories;
using Lykke.Bil2.Sdk.Repositories;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Sdk.BlocksReader.Services
{
    internal class BlockListener : IBlockListener
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IRawObjectWriteOnlyRepository _rawObjectsRepository;

        public BlockListener(
            IMessagePublisher messagePublisher,
            IRawObjectWriteOnlyRepository rawObjectsRepository)
        {
            _messagePublisher = messagePublisher;
            _rawObjectsRepository = rawObjectsRepository;
        }

        public Task HandleHeaderAsync(BlockHeaderReadEvent evt)
        {
            _messagePublisher.Publish(evt);

            return Task.CompletedTask;
        }

        public Task HandleRawBlockAsync(Base58String rawBlock, string blockId)
        {
            return _rawObjectsRepository.SaveAsync(RawObjectType.Block, blockId, rawBlock);
        }

        public Task HandleBlockNotFoundAsync(BlockNotFoundEvent evt)
        {
            _messagePublisher.Publish(evt);

            return Task.CompletedTask;
        }

        public async Task HandleExecutedTransactionAsync(Base58String rawTransaction, TransferAmountTransactionExecutedEvent evt)
        {
            var rawTransactionSavingTask = _rawObjectsRepository.SaveAsync(RawObjectType.Transaction, evt.TransactionId, rawTransaction);

            _messagePublisher.Publish(evt);

            await rawTransactionSavingTask;
        }

        public async Task HandleExecutedTransactionAsync(Base58String rawTransaction, TransferCoinsTransactionExecutedEvent evt)
        {
            var rawTransactionSavingTask = _rawObjectsRepository.SaveAsync(RawObjectType.Transaction, evt.TransactionId, rawTransaction);

            _messagePublisher.Publish(evt);

            await rawTransactionSavingTask;
        }

        public async Task HandleFailedTransactionAsync(Base58String rawTransaction, TransactionFailedEvent evt)
        {
            var rawTransactionSavingTask = _rawObjectsRepository.SaveAsync(RawObjectType.Transaction, evt.TransactionId, rawTransaction);

            _messagePublisher.Publish(evt);

            await rawTransactionSavingTask;
        }
    }
}

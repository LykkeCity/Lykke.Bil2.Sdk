using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.BlocksReader.Events;
using Lykke.Blockchains.Integrations.Contract.Common;
using Lykke.Blockchains.Integrations.RabbitMq;
using Lykke.Blockchains.Integrations.Sdk.BlocksReader.Repositories;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader.Services
{
    internal class BlockListener : IBlockListener
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IRawTransactionWriteOnlyRepository _rawTransactionsRepository;

        public BlockListener(
            IMessagePublisher messagePublisher,
            IRawTransactionWriteOnlyRepository rawTransactionsRepository)
        {
            _messagePublisher = messagePublisher;
            _rawTransactionsRepository = rawTransactionsRepository;
        }

        public Task HandleHeaderAsync(BlockHeaderReadEvent evt)
        {
            _messagePublisher.Publish(evt);

            return Task.CompletedTask;
        }

        public async Task HandleExecutedTransactionAsync(Base58String rawTransaction, TransactionExecutedEvent evt)
        {
            var rawTransactionSavingTask = _rawTransactionsRepository.SaveAsync(evt.TransactionHash, rawTransaction);

            _messagePublisher.Publish(evt);

            await rawTransactionSavingTask;
        }

        public async Task HandleFailedTransactionAsync(Base58String rawTransaction, TransactionFailedEvent evt)
        {
            var rawTransactionSavingTask = _rawTransactionsRepository.SaveAsync(evt.TransactionHash, rawTransaction);

            _messagePublisher.Publish(evt);

            await rawTransactionSavingTask;
        }
    }
}

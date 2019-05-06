using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Commands;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Bil2.Sdk.BlocksReader.Repositories;

namespace Lykke.Bil2.Sdk.BlocksReader.Services
{
    internal class ReadBlockCommandsHandler : IMessageHandler<ReadBlockCommand>
    {
        private readonly IBlockReader _blockReader;
        private readonly IRawObjectWriteOnlyRepository _rawObjectRepository;
        private readonly int _transactionsBatchSize;
        private readonly int _maxTransactionsSavingParallelism;
        private readonly BlockchainTransferModel _transferModel;

        public ReadBlockCommandsHandler(
            IBlockReader blockReader,
            IRawObjectWriteOnlyRepository rawObjectRepository,
            int transactionsBatchSize,
            int maxTransactionsSavingParallelism,
            BlockchainTransferModel transferModel)
        {
            _blockReader = blockReader;
            _rawObjectRepository = rawObjectRepository;
            _transactionsBatchSize = transactionsBatchSize;
            _maxTransactionsSavingParallelism = maxTransactionsSavingParallelism;
            _transferModel = transferModel;
        }

        public async Task<MessageHandlingResult> HandleAsync(ReadBlockCommand command, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            var blockListener = new BlockListener
            (
                replyPublisher,
                _rawObjectRepository,
                _transactionsBatchSize,
                _maxTransactionsSavingParallelism,
                _transferModel
            );

            using (blockListener)
            {
                await _blockReader.ReadBlockAsync(command.BlockNumber, blockListener);
                await blockListener.FlushAsync();
            }

            return MessageHandlingResult.Success();
        }
    }
}

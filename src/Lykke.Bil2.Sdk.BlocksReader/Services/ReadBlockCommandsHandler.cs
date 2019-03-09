using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Commands;
using Lykke.Bil2.RabbitMq.Publication;
using Lykke.Bil2.RabbitMq.Subscription;
using Lykke.Bil2.Sdk.BlocksReader.Repositories;

namespace Lykke.Bil2.Sdk.BlocksReader.Services
{
    internal class ReadBlockCommandsHandler : IMessageHandler<ReadBlockCommand>
    {
        private readonly IBlockReader _blockReader;
        private readonly IRawTransactionWriteOnlyRepository _rawTransactionRepository;

        public ReadBlockCommandsHandler(
            IBlockReader blockReader,
            IRawTransactionWriteOnlyRepository rawTransactionRepository)
        {
            _blockReader = blockReader;
            _rawTransactionRepository = rawTransactionRepository;
        }

        public async Task HandleAsync(ReadBlockCommand command, IMessagePublisher messagePublisher)
        {
            var blockListener = new BlockListener(messagePublisher, _rawTransactionRepository);

            await _blockReader.ReadBlockAsync(command.BlockNumber, blockListener);
        }
    }
}

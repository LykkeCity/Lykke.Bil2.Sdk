using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.BlocksReader.Commands;
using Lykke.Blockchains.Integrations.RabbitMq.Publication;
using Lykke.Blockchains.Integrations.Sdk.BlocksReader.Repositories;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader.Services
{
    internal class ReadBlockCommandsHandler
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

        public async Task Handle(ReadBlockCommand command, IMessagePublisher messagePublisher)
        {
            var blockListener = new BlockListener(messagePublisher, _rawTransactionRepository);

            await _blockReader.ReadBlockAsync(command.BlockNumber, blockListener);
        }
    }
}

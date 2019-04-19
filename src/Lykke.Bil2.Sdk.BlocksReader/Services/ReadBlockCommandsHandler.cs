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
        private readonly IRawObjectWriteOnlyRepository _rawObjectRepository;

        public ReadBlockCommandsHandler(
            IBlockReader blockReader,
            IRawObjectWriteOnlyRepository rawObjectRepository)
        {
            _blockReader = blockReader;
            _rawObjectRepository = rawObjectRepository;
        }

        public async Task<MessageHandlingResult> HandleAsync(ReadBlockCommand command, MessageHeaders headers, IMessagePublisher replyPublisher)
        {
            var blockListener = new BlockListener(replyPublisher, _rawObjectRepository);

            await _blockReader.ReadBlockAsync(command.BlockNumber, blockListener);

            return MessageHandlingResult.Success();
        }
    }
}

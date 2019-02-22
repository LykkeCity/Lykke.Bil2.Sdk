using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Commands;
using Lykke.Bil2.RabbitMq.Publication;

namespace Lykke.Bil2.Client.BlocksReader.Services
{
    internal class BlocksReaderApi : IBlocksReaderApi
    {
        private readonly IMessagePublisher _messagePublisher;

        public BlocksReaderApi(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        }

        public Task SendAsync(ReadBlockCommand command)
        {
            _messagePublisher.Publish(command);

            return Task.CompletedTask;
        }
    }
}

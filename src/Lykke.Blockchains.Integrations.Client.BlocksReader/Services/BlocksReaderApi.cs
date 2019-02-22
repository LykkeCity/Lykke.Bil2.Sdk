using System;
using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.BlocksReader.Commands;
using Lykke.Blockchains.Integrations.RabbitMq;

namespace Lykke.Blockchains.Integrations.Client.BlocksReader.Services
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

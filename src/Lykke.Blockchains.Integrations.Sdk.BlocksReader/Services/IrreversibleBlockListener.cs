using System;
using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.Contract.BlocksReader.Events;
using Lykke.Blockchains.Integrations.RabbitMq;

namespace Lykke.Blockchains.Integrations.Sdk.BlocksReader.Services
{
    internal class IrreversibleBlockListener : IIrreversibleBlockListener
    {
        private readonly Func<IMessagePublisher> _messagePublisher;

        public IrreversibleBlockListener(Func<IMessagePublisher> messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public Task HandleNewLastIrreversableBlockAsync(LastIrreversibleBlockUpdatedEvent evt)
        {
            _messagePublisher.Invoke().Publish(evt);

            return Task.CompletedTask;
        }
    }
}

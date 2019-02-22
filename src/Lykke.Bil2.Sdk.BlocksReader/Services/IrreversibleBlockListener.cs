using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.BlocksReader.Events;
using Lykke.Bil2.RabbitMq.Publication;

namespace Lykke.Bil2.Sdk.BlocksReader.Services
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

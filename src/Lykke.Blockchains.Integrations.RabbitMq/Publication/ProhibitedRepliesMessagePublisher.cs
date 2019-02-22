using System;

namespace Lykke.Blockchains.Integrations.RabbitMq.Publication
{
    internal class ProhibitedRepliesMessagePublisher : IMessagePublisher
    {
        public static readonly IMessagePublisher Instance = new ProhibitedRepliesMessagePublisher();

        public void Publish<TMessage>(TMessage message, string correlationId = null)
        {
            throw new InvalidOperationException("Replies are prohibited for this subscription");
        }
    }
}

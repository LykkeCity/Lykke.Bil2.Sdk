using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.RabbitMq.Publication
{
    /// <summary>
    /// Publishes messages to the RabbitMq.
    /// </summary>
    [PublicAPI]
    public interface IMessagePublisher
    {
        /// <summary>
        /// Publishes messages to the RabbitMq.
        /// </summary>
        void Publish<TMessage>(TMessage message, string correlationId = null);
    }
}

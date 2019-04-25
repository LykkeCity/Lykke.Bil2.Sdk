using JetBrains.Annotations;

namespace Lykke.Bil2.RabbitMq.Publication
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

        /// <summary>
        /// Creates new message publisher with specified correlation id
        /// </summary>
        IMessagePublisher ChangeCorrelationId(string correlationId);
    }
}

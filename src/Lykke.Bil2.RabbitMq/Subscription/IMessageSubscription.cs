using System;
using System.Threading.Tasks;
using Lykke.Bil2.RabbitMq.Publication;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    /// <summary>
    /// Metadata of the message subscription
    /// </summary>
    public interface IMessageSubscription
    {
        /// <summary>
        /// Message unique RabbitMq routing key
        /// </summary>
        string RoutingKey { get; }

        /// <summary>
        /// CLR type of the message
        /// </summary>
        Type MessageType { get; }

        /// <summary>
        /// Invokes the message handler
        /// </summary>
        Task InvokeHandlerAsync(object message, IMessagePublisher publisher);
    }
}

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.RabbitMq.Publication;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    /// <summary>
    /// Metadata of the message subscription
    /// </summary>
    [PublicAPI]
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
        Task<MessageHandlingResult> InvokeHandlerAsync(
            IServiceProvider parentServiceProvider,
            object message,
            MessageHeaders headers,
            MessageHandlingContext handlingContext, 
            IMessagePublisher publisher);
    }
}

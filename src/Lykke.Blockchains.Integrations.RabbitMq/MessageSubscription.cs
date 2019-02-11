using System;
using System.Threading.Tasks;

namespace Lykke.Blockchains.Integrations.RabbitMq
{
    /// <summary>
    /// Metadata of the message subscription
    /// </summary>
    public class MessageSubscription
    {
        public string RoutingKey { get; }

        public Type MessageType { get; }

        private readonly Delegate _handler;

        /// <summary>
        /// Metadata of the message subscription
        /// </summary>
        public MessageSubscription(string routingKey, Type messageType, Delegate handler)
        {
            if (string.IsNullOrWhiteSpace(routingKey))
            {
                throw new ArgumentException(nameof(routingKey));
            }

            RoutingKey = routingKey;
            MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));

            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        /// Invokes the message handler
        /// </summary>
        public async Task InvokeHandlerAsync(object message, IMessagePublisher publisher)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }

            await (Task)_handler.DynamicInvoke(message, publisher);
        }
    }
}

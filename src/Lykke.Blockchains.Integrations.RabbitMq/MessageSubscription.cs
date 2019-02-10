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
            RoutingKey = routingKey;
            MessageType = messageType;

            _handler = handler;
        }

        /// <summary>
        /// Invokes the message handler
        /// </summary>
        public async Task InvokeHandlerAsync(object message, IMessagePublisher publisher)
        {
            await (Task)_handler.DynamicInvoke(message, publisher);
        }
    }
}

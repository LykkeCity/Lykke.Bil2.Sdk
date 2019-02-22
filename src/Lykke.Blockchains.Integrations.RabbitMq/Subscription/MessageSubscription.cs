using System;
using System.Threading.Tasks;
using Lykke.Blockchains.Integrations.RabbitMq.Publication;

namespace Lykke.Blockchains.Integrations.RabbitMq.Subscription
{
    internal class MessageSubscription<TMessage> : IMessageSubscription
    {
        public string RoutingKey { get; }

        public Type MessageType { get; }

        private readonly Func<TMessage, IMessagePublisher, Task> _handler;

        /// <summary>
        /// Metadata of the message subscription
        /// </summary>
        public MessageSubscription(string routingKey, Type messageType, Func<TMessage, IMessagePublisher, Task> handler)
        {
            if (string.IsNullOrWhiteSpace(routingKey))
            {
                throw new ArgumentException(nameof(routingKey));
            }

            RoutingKey = routingKey;
            MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));

            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

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

            if (message is TMessage typedMessage)
            {
                await _handler.Invoke(typedMessage, publisher);
            }
            else
            {
                throw new ArgumentException($"Object of type {typeof(TMessage)} is expected, but {message.GetType()} is passed", nameof(message));
            }
        }
    }
}

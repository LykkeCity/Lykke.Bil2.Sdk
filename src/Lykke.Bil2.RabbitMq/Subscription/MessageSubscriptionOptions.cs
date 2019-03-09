using System;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    internal class MessageSubscriptionOptions<TMessage> : IMessageSubscriptionOptions<TMessage> 
        where TMessage : class
    {
        public Type MessageType { get; }
        public string RoutingKey { get; }
        public Type HandlerType { get; private set; }

        public MessageSubscriptionOptions(string routingKey)
        {
            if (string.IsNullOrWhiteSpace(routingKey))
            {
                throw new ArgumentException(nameof(routingKey));
            }

            MessageType = typeof(TMessage);
            RoutingKey = routingKey;
        }

        public void WithHandler<THandler>()
            where THandler : IMessageHandler<TMessage>
        {
            HandlerType = typeof(THandler);
        }
    }

    internal class MessageSubscriptionOptions<TMessage, TState> : IMessageSubscriptionOptions<TMessage, TState> 
        where TMessage : class
    {
        public Type MessageType { get; }
        public string RoutingKey { get; }
        public Type HandlerType { get; private set; }
        public TState State { get; private set; }

        public MessageSubscriptionOptions(string routingKey)
        {
            if (string.IsNullOrWhiteSpace(routingKey))
            {
                throw new ArgumentException(nameof(routingKey));
            }

            MessageType = typeof(TMessage);
            RoutingKey = routingKey;
        }

        public void WithHandler<THandler>()
            where THandler : IMessageHandler<TMessage, TState>
        {
            HandlerType = typeof(THandler);
        }

        public void WithState(TState state)
        {
            State = state;
        }
    }
}

using System;
using System.Threading.Tasks;
using Lykke.Bil2.RabbitMq.Publication;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    internal class MessageSubscription<TMessage> : IMessageSubscription
        where TMessage : class
    {
        public string RoutingKey => _options.RoutingKey;

        public Type MessageType => _options.MessageType;

        private readonly MessageSubscriptionOptions<TMessage> _options;

        /// <summary>
        /// Metadata of the message subscription
        /// </summary>
        public MessageSubscription(MessageSubscriptionOptions<TMessage> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task InvokeHandlerAsync(
            IServiceProvider parentServiceProvider, 
            object message,
            MessageHeaders headers,
            IMessagePublisher publisher)
        {
            if (parentServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(parentServiceProvider));
            }
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
                using (var scope = parentServiceProvider.CreateScope())
                {
                    var handlers = (IMessageHandler<TMessage>) scope.ServiceProvider.GetRequiredService(_options.HandlerType);

                    await handlers.HandleAsync(typedMessage, headers, publisher);
                }
            }
            else
            {
                throw new ArgumentException($"Object of type {typeof(TMessage)} is expected, but {message.GetType()} is passed", nameof(message));
            }
        }
    }

    internal class MessageSubscription<TMessage, TState> : IMessageSubscription
        where TMessage : class
    {
        public string RoutingKey => _options.RoutingKey;

        public Type MessageType => _options.MessageType;

        private readonly MessageSubscriptionOptions<TMessage, TState> _options;

        /// <summary>
        /// Metadata of the message subscription
        /// </summary>
        public MessageSubscription(MessageSubscriptionOptions<TMessage, TState> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task InvokeHandlerAsync(
            IServiceProvider parentServiceProvider, 
            object message,
            MessageHeaders headers,
            IMessagePublisher publisher)
        {
            if (parentServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(parentServiceProvider));
            }
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
                using (var scope = parentServiceProvider.CreateScope())
                {
                    var handlers = (IMessageHandler<TMessage, TState>) scope.ServiceProvider.GetRequiredService(_options.HandlerType);

                    await handlers.HandleAsync(_options.State, typedMessage, headers, publisher);
                }
            }
            else
            {
                throw new ArgumentException($"Object of type {typeof(TMessage)} is expected, but {message.GetType()} is passed", nameof(message));
            }
        }
    }
}

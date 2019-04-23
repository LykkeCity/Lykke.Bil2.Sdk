using System;
using System.Collections.Generic;
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
        private readonly IReadOnlyCollection<IMessageFilter> _globalFilters;

        /// <summary>
        /// Metadata of the message subscription
        /// </summary>
        public MessageSubscription(
            MessageSubscriptionOptions<TMessage> options,
            IReadOnlyCollection<IMessageFilter> globalFilters)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _globalFilters = globalFilters ?? throw new ArgumentNullException(nameof(globalFilters));
        }

        public Task<MessageHandlingResult> InvokeHandlerAsync(
            IServiceProvider parentServiceProvider, 
            object message,
            MessageHeaders headers,
            MessageHandlingContext handlingContext,
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
                    var handler = (IMessageHandler<TMessage>) scope.ServiceProvider.GetRequiredService(_options.HandlerType);
                    var context = new MessageFilteringContext
                    (
                        _globalFilters.GetEnumerator(),
                        null,
                        typedMessage,
                        headers,
                        handlingContext,
                        () => handler.HandleAsync(typedMessage, headers, publisher)
                    );
                    
                    return context.InvokeNextAsync();
                }
            }

            throw new ArgumentException($"Object of type {typeof(TMessage)} is expected, but {message.GetType()} is passed", nameof(message));
        }
    }

    internal class MessageSubscription<TMessage, TState> : IMessageSubscription
        where TMessage : class
    {
        public string RoutingKey => _options.RoutingKey;

        public Type MessageType => _options.MessageType;

        private readonly MessageSubscriptionOptions<TMessage, TState> _options;
        private readonly IReadOnlyCollection<IMessageFilter> _globalFilters;

        /// <summary>
        /// Metadata of the message subscription
        /// </summary>
        public MessageSubscription(
            MessageSubscriptionOptions<TMessage, TState> options,
            IReadOnlyCollection<IMessageFilter> globalFilters)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _globalFilters = globalFilters ?? throw new ArgumentNullException(nameof(globalFilters));
        }

        public Task<MessageHandlingResult> InvokeHandlerAsync(IServiceProvider parentServiceProvider,
            object message,
            MessageHeaders headers,
            MessageHandlingContext handlingContext,
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
                    var handler = (IMessageHandler<TMessage, TState>) scope.ServiceProvider.GetRequiredService(_options.HandlerType);

                    var context = new MessageFilteringContext
                    (
                        _globalFilters.GetEnumerator(),
                        _options.State,
                        typedMessage,
                        headers,
                        handlingContext,
                        () => handler.HandleAsync(_options.State, typedMessage, headers, publisher)
                    );
                    
                    return context.InvokeNextAsync();
                }
            }

            throw new ArgumentException($"Object of type {typeof(TMessage)} is expected, but {message.GetType()} is passed", nameof(message));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    /// <inheritdoc />
    [PublicAPI]
    public class MessageSubscriptionsRegistry : IMessageSubscriptionsRegistry
    {
        private IImmutableDictionary<string, IMessageSubscription> _subscriptions;

        /// <summary>
        /// Registry of the message subscriptions
        /// </summary>
        public MessageSubscriptionsRegistry()
        {
            _subscriptions = new Dictionary<string, IMessageSubscription>().ToImmutableDictionary();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IMessageSubscription> GetAllSubscriptions()
        {
            return _subscriptions.Values.ToArray();
        }

        /// <inheritdoc />
        public IMessageSubscription GetSubscriptionOrDefault(string messageType)
        {
            if (string.IsNullOrWhiteSpace(messageType))
            {
                throw new ArgumentException("Should be not empty string", nameof(messageType));
            }

            _subscriptions.TryGetValue(messageType, out var subscription);

            return subscription;
        }

        /// <inheritdoc />
        public MessageSubscriptionsRegistry Handle<TMessage>(Action<IMessageSubscriptionOptions<TMessage>> configureSubscription)
            where TMessage : class
        {
            if (configureSubscription == null)
            {
                throw new ArgumentNullException(nameof(configureSubscription));
            }

            var messageType = typeof(TMessage);
            var subscriptionOptions = new MessageSubscriptionOptions<TMessage>(messageType.Name);

            configureSubscription.Invoke(subscriptionOptions);

            if (subscriptionOptions.HandlerType == null)
            {
                throw new InvalidOperationException($"Handler is not registered for the message {messageType.Name}.");
            }
            
            _subscriptions = _subscriptions.Add(messageType.Name, new MessageSubscription<TMessage>(subscriptionOptions));

            return this;
        }

        /// <inheritdoc />
        public MessageSubscriptionsRegistry Handle<TMessage, TState>(Action<IMessageSubscriptionOptions<TMessage, TState>> configureSubscription)
            where TMessage : class
        {
            if (configureSubscription == null)
            {
                throw new ArgumentNullException(nameof(configureSubscription));
            }

            var messageType = typeof(TMessage);
            var subscriptionOptions = new MessageSubscriptionOptions<TMessage, TState>(messageType.Name);

            configureSubscription.Invoke(subscriptionOptions);

            if (subscriptionOptions.HandlerType == null)
            {
                throw new InvalidOperationException($"Handler is not registered for the message {messageType.Name}.");
            }
           
            _subscriptions = _subscriptions.Add(messageType.Name, new MessageSubscription<TMessage, TState>(subscriptionOptions));

            return this;
        }
    }
}

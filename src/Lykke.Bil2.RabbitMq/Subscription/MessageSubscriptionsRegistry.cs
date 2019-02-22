using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.RabbitMq.Publication;

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
        public MessageSubscriptionsRegistry On<TMessage>(Func<TMessage, IMessagePublisher, Task> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var type = typeof(TMessage);

            _subscriptions = _subscriptions.Add(type.Name, new MessageSubscription<TMessage>(type.Name, type, handler));

            return this;
        }
    }
}

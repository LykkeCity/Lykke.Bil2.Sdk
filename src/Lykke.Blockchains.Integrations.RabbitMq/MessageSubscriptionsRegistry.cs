using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.RabbitMq
{
    /// <inheritdoc />
    [PublicAPI]
    public class MessageSubscriptionsRegistry : IMessageSubscriptionsRegistry
    {
        private IImmutableDictionary<string, MessageSubscription> _subscriptions;

        /// <summary>
        /// Registry of the message subscriptions
        /// </summary>
        public MessageSubscriptionsRegistry()
        {
            _subscriptions = new Dictionary<string, MessageSubscription>().ToImmutableDictionary();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<MessageSubscription> GetAllSubscriptions()
        {
            return _subscriptions.Values.ToArray();
        }

        /// <inheritdoc />
        public MessageSubscription GetSubscriptionOrDefault(string messageType)
        {
            _subscriptions.TryGetValue(messageType, out var subscription);

            return subscription;
        }

        /// <inheritdoc />
        public MessageSubscriptionsRegistry On<TMessage>(Func<TMessage, IMessagePublisher, Task> handler)
        {
            var type = typeof(TMessage);

            _subscriptions = _subscriptions.Add(type.Name, new MessageSubscription(type.Name, type, handler));

            return this;
        }
    }
}

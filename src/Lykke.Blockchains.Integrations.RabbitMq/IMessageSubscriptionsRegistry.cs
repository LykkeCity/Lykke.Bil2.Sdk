using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.RabbitMq
{
    /// <summary>
    /// Registry of the message subscriptions
    /// </summary>
    [PublicAPI]
    public interface IMessageSubscriptionsRegistry
    {
        /// <summary>
        /// Returns all registered subscriptions
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<IMessageSubscription> GetAllSubscriptions();

        /// <summary>
        /// Returns subscription by the message type or null
        /// </summary>
        IMessageSubscription GetSubscriptionOrDefault(string messageType);

        /// <summary>
        /// Register handler for the message
        /// </summary>
        MessageSubscriptionsRegistry On<TMessage>(Func<TMessage, IMessagePublisher, Task> handler);
    }
}

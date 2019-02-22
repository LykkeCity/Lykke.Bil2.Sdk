using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Blockchains.Integrations.RabbitMq.Publication;

namespace Lykke.Blockchains.Integrations.RabbitMq.Subscription
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

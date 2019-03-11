using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Bil2.RabbitMq.Subscription
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
        /// Configures handling of the <typeparamref name="TMessage"/> message.
        /// </summary>
        MessageSubscriptionsRegistry Handle<TMessage>(Action<IMessageSubscriptionOptions<TMessage>> configureSubscription)
            where TMessage : class;

        /// <summary>
        /// Configures handling of the <typeparamref name="TMessage"/> message with the <typeparamref name="TState"/> state.
        /// </summary>
        MessageSubscriptionsRegistry Handle<TMessage, TState>(Action<IMessageSubscriptionOptions<TMessage, TState>> configureSubscription)
            where TMessage : class;
    }
}

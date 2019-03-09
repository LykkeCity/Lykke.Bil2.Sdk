using JetBrains.Annotations;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    /// <summary>
    /// Options of the message <typeparamref name="TMessage"/> handling.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    [PublicAPI]
    public interface IMessageSubscriptionOptions<out TMessage> 
        where TMessage : class
    {
        /// <summary>
        /// Specifies a handler to handle the <typeparamref name="TMessage"/>.
        /// </summary>
        void WithHandler<THandler>() where THandler : IMessageHandler<TMessage>;
    }

    /// <summary>
    /// Options of the message <typeparamref name="TMessage"/> handling.
    /// </summary>
    [PublicAPI]
    public interface IMessageSubscriptionOptions<out TMessage, TState> 
        where TMessage : class
    {
        /// <summary>
        /// Specifies a handler to handle the <typeparamref name="TMessage"/>.
        /// </summary>
        void WithHandler<THandler>() where THandler : IMessageHandler<TMessage, TState>;

        /// <summary>
        /// Specifies a state which will be passed to the handler.
        /// </summary>
        void WithState(TState state);
    }
}

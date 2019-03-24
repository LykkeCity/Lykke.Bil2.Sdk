using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Bil2.RabbitMq.Publication;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    /// <summary>
    /// RabbitMq message handler.
    /// </summary>
    [PublicAPI]
    public interface IMessageHandler<in TMessage>
        where TMessage : class
    {
        Task HandleAsync(TMessage message, MessageHeaders headers, IMessagePublisher replyPublisher);
    }

    /// <summary>
    /// RabbitMq message handler with custom state.
    /// </summary>
    [PublicAPI]
    public interface IMessageHandler<in TMessage, in TState>
        where TMessage : class
    {
        Task HandleAsync(TState state, TMessage message, MessageHeaders headers, IMessagePublisher replyPublisher);
    }
}

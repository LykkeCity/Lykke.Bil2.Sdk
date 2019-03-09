using System.Threading.Tasks;
using Lykke.Bil2.RabbitMq.Publication;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    /// <summary>
    /// RabbitMq message handler.
    /// </summary>
    public interface IMessageHandler<in TMessage>
        where TMessage : class
    {
        Task HandleAsync(TMessage message, IMessagePublisher publisher);
    }

    /// <summary>
    /// RabbitMq message handler with custom state.
    /// </summary>
    public interface IMessageHandler<in TMessage, in TState>
        where TMessage : class
    {
        Task HandleAsync(TState state, TMessage message, IMessagePublisher publisher);
    }
}

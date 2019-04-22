using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    /// <summary>
    /// Message filter, which allows to wrap message processing with some custom logic.
    /// </summary>
    [PublicAPI]
    public interface IMessageFilter
    {
        Task<MessageHandlingResult> HandleMessageAsync(MessageFilteringContext context);
    }
}

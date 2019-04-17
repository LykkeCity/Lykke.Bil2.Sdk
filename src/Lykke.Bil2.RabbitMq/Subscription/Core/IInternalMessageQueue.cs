using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal interface IInternalMessageQueue
    {
        void Enqueue(
            EnvelopedMessage message);

        Task EnqueueAsync(
            EnvelopedMessage message,
            CancellationToken cancellationToken);

        Task<EnvelopedMessage> DequeueAsync(
            CancellationToken cancellationToken);
    }
}

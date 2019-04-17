using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal class InternalMessageQueue : IInternalMessageQueue
    {
        private readonly AsyncProducerConsumerQueue<EnvelopedMessage> _innerQueue;

        public InternalMessageQueue(
            int maxCapacity)
        {
            _innerQueue = new AsyncProducerConsumerQueue<EnvelopedMessage>(maxCapacity);
        }
        
        public void Enqueue(
            EnvelopedMessage message)
        {
            _innerQueue.Enqueue(message);
        }

        public Task EnqueueAsync(
            EnvelopedMessage message,
            CancellationToken cancellationToken)
        {
            return _innerQueue.EnqueueAsync(message, cancellationToken);
        }

        public Task<EnvelopedMessage> DequeueAsync(
            CancellationToken cancellationToken)
        {
            return _innerQueue.DequeueAsync(cancellationToken);
        }
    }
}

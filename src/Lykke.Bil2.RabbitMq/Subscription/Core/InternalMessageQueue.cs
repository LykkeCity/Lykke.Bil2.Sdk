using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal class InternalMessageQueue : IInternalMessageQueue
    {
        public bool IsFull => _currentCount > _maxCapacity;

        private readonly AsyncProducerConsumerQueue<EnvelopedMessage> _innerQueue;
        private readonly int _maxCapacity;
        private int _currentCount;

        public InternalMessageQueue(
            int maxCapacity)
        {
            _innerQueue = new AsyncProducerConsumerQueue<EnvelopedMessage>(maxCapacity);
            _maxCapacity = maxCapacity;
        }
        
        public void Enqueue(
            EnvelopedMessage message)
        {
            _innerQueue.Enqueue(message);

            Interlocked.Increment(ref _currentCount);
        }

        public async Task EnqueueAsync(
            EnvelopedMessage message,
            CancellationToken cancellationToken)
        {
            await _innerQueue.EnqueueAsync(message, cancellationToken);

            Interlocked.Increment(ref _currentCount);
        }

        public async Task<EnvelopedMessage> DequeueAsync(
            CancellationToken cancellationToken)
        {
            var message = await _innerQueue.DequeueAsync(cancellationToken);

            Interlocked.Decrement(ref _currentCount);

            return message;
        }
    }
}

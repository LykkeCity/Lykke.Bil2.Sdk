using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using C5;
using Common.Log;
using Lykke.Common.Log;
using Nito.AsyncEx;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal class RetryManager : BackgroundService, IRetryManager
    {
        private readonly IPriorityQueue<(DateTime, EnvelopedMessage)> _firstLevelRetryQueue;
        private readonly ILog _log;
        private readonly TimeSpan _maxFirstLevelRetryMessageAge;
        private readonly int _maxFirstLevelRetryCount;
        private readonly int _firstLevelRetryQueueCapacity;
        private readonly AsyncLock _mutex;
        private readonly IInternalMessageQueue _internalQueue;

        public RetryManager(
            ILogFactory logFactory,
            TimeSpan maxFirstLevelRetryMessageAge,
            int maxFirstLevelRetryCount,
            int firstLevelRetryQueueCapacity,
            IInternalMessageQueue internalQueue) : base(logFactory)
        {
            _firstLevelRetryQueue = new IntervalHeap<(DateTime, EnvelopedMessage)>(new Comparer());
            _log = logFactory.CreateLog(this);
            _maxFirstLevelRetryMessageAge = maxFirstLevelRetryMessageAge;
            _maxFirstLevelRetryCount = maxFirstLevelRetryCount;
            _firstLevelRetryQueueCapacity = firstLevelRetryQueueCapacity;
            _mutex = new AsyncLock();
            _internalQueue = internalQueue;
        }

        public override void Start()
        {
            base.Start();
            
            _log.Info("Retry manager has been started.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            
            _log.Info("Retry manager has been stopped.");
        }
        
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && !_firstLevelRetryQueue.IsEmpty)
            {
                EnvelopedMessage messageToRetry;
                
                using (await _mutex.LockAsync())
                {
                    var (timeToRetry, message) = _firstLevelRetryQueue.FindMin();

                    if (DateTime.UtcNow < timeToRetry)
                    {
                        break;
                    }

                    _firstLevelRetryQueue.DeleteMin();

                    if (message.Age <= _maxFirstLevelRetryMessageAge)
                    {
                        messageToRetry = message.WithIncreasedRetryCount();
                    }
                    else
                    {
                        message.Reject();
                        
                        _log.Trace("Message has been rejected due to expiration.", message);
                        
                        continue;
                    }
                }

                if (_internalQueue.IsFull)
                {
                    messageToRetry.Reject();

                    _log.Warning("Message has been rejected due to full internal queue.", context: messageToRetry);
                }
                else
                {
                    await _internalQueue.EnqueueAsync(messageToRetry, stoppingToken);

                    _log.Trace("Message has been re-enqueued to internal queue.", messageToRetry);
                }
            }

            await SilentlyDelayAsync(100, stoppingToken);
        }

        public void ScheduleRetry(
            EnvelopedMessage message,
            TimeSpan retryAfter)
        {
            if (message.RetryCount < _maxFirstLevelRetryCount)
            {
                using (_mutex.Lock())
                {
                    var timeToRetry = DateTime.UtcNow.Add(retryAfter);

                    if (_firstLevelRetryQueue.Count < _firstLevelRetryQueueCapacity)
                    {
                        _firstLevelRetryQueue.Add((timeToRetry, message));
                    
                        _log.Trace($"Message retry has been scheduled for {timeToRetry}.", message);
                    }
                    else
                    {
                        message.Reject();

                        _log.Warning("First level retry queue is full. Message has been rejected,", context: message);
                    }
                }
            }
            else
            {
                message.Reject();
                
                _log.Trace($"Max retry count [{_maxFirstLevelRetryCount}] for message has been exceeded.", message);
            }
        }

        private class Comparer : IComparer<(DateTime, EnvelopedMessage)>
        {
            public int Compare(
                (DateTime, EnvelopedMessage) x, 
                (DateTime, EnvelopedMessage) y)
            {
                return x.Item1.CompareTo(y.Item1);
            }
        }
    }
}

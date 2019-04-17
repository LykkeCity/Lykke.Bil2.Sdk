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
        private readonly ILog _log;
        private readonly TimeSpan _maxAgeForRetry;
        private readonly int _maxRetryCount;
        private readonly AsyncLock _mutex;
        private readonly IPriorityQueue<(DateTime, EnvelopedMessage)> _retryQueue;
        private readonly IInternalMessageQueue _internalQueue;

        public RetryManager(
            ILogFactory logFactory,
            TimeSpan maxAgeForRetry,
            int maxRetryCount,
            IInternalMessageQueue internalQueue)
        {
            _log = logFactory.CreateLog(this);
            _maxAgeForRetry = maxAgeForRetry;
            _maxRetryCount = maxRetryCount;
            _mutex = new AsyncLock();
            _retryQueue = new IntervalHeap<(DateTime, EnvelopedMessage)>(new Comparer());
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
            while (!stoppingToken.IsCancellationRequested)
            {
                while (!stoppingToken.IsCancellationRequested && !_retryQueue.IsEmpty)
                {
                    EnvelopedMessage messageToRetry;
                    
                    using (await _mutex.LockAsync())
                    {
                        var (timeToRetry, message) = _retryQueue.FindMin();

                        if (DateTime.UtcNow < timeToRetry)
                        {
                            break;
                        }
                        else
                        {
                            _retryQueue.DeleteMin();
                        }
                        
                        if (message.Age <= _maxAgeForRetry)
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

                    await _internalQueue.EnqueueAsync(messageToRetry, stoppingToken);
                    
                    _log.Trace("Message has been re-enqueued to internal queue.", messageToRetry);
                }

                await SilentlyDelayAsync(100, stoppingToken);
            }
        }

        public void ScheduleRetry(
            EnvelopedMessage message,
            TimeSpan retryAfter)
        {
            if (message.RetryCount < _maxRetryCount)
            {
                using (_mutex.Lock())
                {
                    var timeToRetry = DateTime.UtcNow.Add(retryAfter);
                    
                    _retryQueue.Add((timeToRetry, message));
                    
                    _log.Trace($"Message retry has been scheduled for {timeToRetry}.", message);
                }
            }
            else
            {
                message.Reject();
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

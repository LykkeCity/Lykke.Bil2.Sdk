using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using C5;
using Common.Log;
using Lykke.Common.Log;
using Nito.AsyncEx;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal class RejectManager : BackgroundService, IRejectManager
    {
        private readonly ILog _log;
        private readonly AsyncLock _mutex;
        private readonly IPriorityQueue<(DateTime, EnvelopedMessage)> _rejectionQueue;
        private readonly Func<DateTime> _utcNowProvider;


        public RejectManager(
            ILogFactory logFactory,
            Func<DateTime> utcNowProvider = null)
        {
            _log = logFactory.CreateLog(this);
            _mutex = new AsyncLock();
            _rejectionQueue = new IntervalHeap<(DateTime, EnvelopedMessage)>(new Comparer());

            if (utcNowProvider == null)
            {
                _utcNowProvider = () => DateTime.UtcNow;
            }
            else
            {
                _utcNowProvider = utcNowProvider;
            }
        }
        
        public override void Start()
        {
            base.Start();
            
            _log.Info("Reject manager has been started.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            
            _log.Info("Reject manager has been stopped.");
        }
        
        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                while (!stoppingToken.IsCancellationRequested && !_rejectionQueue.IsEmpty)
                {
                    using (await _mutex.LockAsync())
                    {
                        var (timeToReject, message) = _rejectionQueue.FindMin();
                        
                        if (_utcNowProvider.Invoke() >= timeToReject)
                        {
                            message.Reject();

                            _rejectionQueue.DeleteMin();
                            
                            _log.Trace("Message has been rejected.", message);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                await SilentlyDelayAsync(100, stoppingToken);
            }
        }

        public void ScheduleReject(
            EnvelopedMessage message,
            TimeSpan rejectAfter)
        {
            using (_mutex.Lock())
            {
                var timeToReject = _utcNowProvider.Invoke().Add(rejectAfter);
                
                _rejectionQueue.Add((timeToReject, message));
                
                _log.Trace($"Message rejection has been scheduled for {timeToReject}.", message);
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

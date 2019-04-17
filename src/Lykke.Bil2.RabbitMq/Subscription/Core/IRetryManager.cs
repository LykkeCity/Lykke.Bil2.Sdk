using System;
using System.Threading.Tasks;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal interface IRetryManager : IBackgroundService
    {
        void ScheduleRetry(
            EnvelopedMessage message,
            TimeSpan retryAfter);
    }
}

using System;

namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal interface IRejectManager : IBackgroundService
    {
        void ScheduleReject(
            EnvelopedMessage message,
            TimeSpan rejectAfter);
    }
}

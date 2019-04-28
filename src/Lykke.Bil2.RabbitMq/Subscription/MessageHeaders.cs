using System;
using JetBrains.Annotations;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    /// <summary>
    /// Headers of the message.
    /// </summary>
    [PublicAPI]
    public class MessageHeaders
    {
        public string CorrelationId { get; }
        public DateTime PublishedAt { get; }

        public MessageHeaders(string correlationId, DateTime publishedAt)
        {
            CorrelationId = correlationId;
            PublishedAt = publishedAt;
        }
    }
}

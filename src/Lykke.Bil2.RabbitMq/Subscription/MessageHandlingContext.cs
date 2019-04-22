using JetBrains.Annotations;

namespace Lykke.Bil2.RabbitMq.Subscription
{
    /// <summary>
    /// Context of the message handling.
    /// </summary>
    [PublicAPI]
    public class MessageHandlingContext
    {
        public string Exchange { get; }
        public int RetryCount { get; }
        public string RoutingKey { get; }

        internal MessageHandlingContext(string exchange, int retryCount, string routingKey)
        {
            Exchange = exchange;
            RetryCount = retryCount;
            RoutingKey = routingKey;
        }
    }
}

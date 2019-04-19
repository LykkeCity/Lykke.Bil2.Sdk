namespace Lykke.Bil2.RabbitMq.Subscription.Core
{
    internal interface IMessageConsumer
    {
        void Ack(
            ulong deliveryTag);

        void Reject(
            ulong deliveryTag);

        void Start();

        void Stop();
    }
}

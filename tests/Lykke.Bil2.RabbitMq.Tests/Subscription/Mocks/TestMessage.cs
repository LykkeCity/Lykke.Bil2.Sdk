using System;
using MessagePack;

namespace Lykke.Bil2.RabbitMq.Tests.Subscription.Mocks
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class TestMessage
    {
        public Guid Id { get; set; }
    }
}

using System;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;
using MessagePack;

namespace Lykke.Bil2.RabbitMq.Tests.MessagePack.Mocks
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class TestContractObject
    {
        public Guid Id { get; set; }
        public Money Amount { get; set; }
        public DateTime DateTime { get; set; }
        public int Number { get; set; }
        public BlockId BlockId { get; set; }
        public decimal Decimal { get; set; }
    }
}

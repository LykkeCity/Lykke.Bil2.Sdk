using System;
using System.Globalization;
using Lykke.Bil2.RabbitMq.MessagePack;
using Lykke.Bil2.RabbitMq.Tests.MessagePack.Mocks;
using Lykke.Numerics;
using Lykke.Numerics.MessagePack;
using MessagePack;
using NUnit.Framework;

namespace Lykke.Bil2.RabbitMq.Tests.MessagePack
{
    [TestFixture]
    public class MessagePackSerializationTests
    {
        [Test]
        public void Test_that_serialization_is_culture_insensitive()
        {
            var formatterResolver = new CompositeFormatterResolver();
          
            formatterResolver.RegisterResolvers(MoneyResolver.Instance);

            var obj = new TestContractObject
            {
                Id = Guid.NewGuid(),
                Amount = new Money(123, 1),
                DateTime = DateTime.UtcNow,
                Number = 111,
                BlockId = "block-id",
                Decimal = 123.123M
            };

            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;

            var objBytes = MessagePackSerializer.Serialize(obj, formatterResolver);

            CultureInfo.CurrentCulture = new CultureInfo("us-EN");
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;

            var deserializedObj = MessagePackSerializer.NonGeneric.Deserialize(typeof(TestContractObject), objBytes, formatterResolver);

            Assert.IsInstanceOf<TestContractObject>(deserializedObj);

            var typedDeserializedObj = (TestContractObject) deserializedObj;

            Assert.AreEqual(typedDeserializedObj.Id, obj.Id);
            Assert.AreEqual(typedDeserializedObj.BlockId, obj.BlockId);
            Assert.AreEqual(typedDeserializedObj.Amount, obj.Amount);
            Assert.AreEqual(typedDeserializedObj.DateTime, obj.DateTime);
            Assert.AreEqual(typedDeserializedObj.Number, obj.Number);
            Assert.AreEqual(typedDeserializedObj.Decimal, obj.Decimal);
        }
    }
}

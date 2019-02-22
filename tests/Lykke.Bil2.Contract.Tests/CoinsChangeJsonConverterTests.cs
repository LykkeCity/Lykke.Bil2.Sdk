using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class CoinsChangeJsonConverterTests
    {
        private class TypeWithCoinsChangeProperty
        {
            public CoinsChange Amount { get; set; }
        }

        [Test]
        [TestCase("-10.1230", ExpectedResult = "{\"Amount\":\"-10.1230\"}")]
        public string Can_be_serailized(string stringValue)
        {
            var obj = new TypeWithCoinsChangeProperty
            {
                Amount = CoinsChange.Create(stringValue)
            };

            return JsonConvert.SerializeObject(obj);
        }

        [Test]
        [TestCase("{\"Amount\":\"-0.553000\"}", ExpectedResult = "-0.553000")]
        public string Can_be_deserailized(string json)
        {
            var obj = JsonConvert.DeserializeObject<TypeWithCoinsChangeProperty>(json);

            return obj.Amount.StringValue;
        }

        [Test]
        public void Null_can_be_deserailized()
        {
            var obj = JsonConvert.DeserializeObject<TypeWithCoinsChangeProperty>("{\"Amount\":null}");

            Assert.IsNull(obj.Amount);
        }
    }
}

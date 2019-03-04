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
        public string Can_be_serialized(string stringValue)
        {
            var obj = new TypeWithCoinsChangeProperty
            {
                Amount = CoinsChange.Parse(stringValue)
            };

            return JsonConvert.SerializeObject(obj);
        }

        [Test]
        [TestCase("{\"Amount\":\"-0.553000\"}", ExpectedResult = "-0.553000")]
        public string Can_be_deserialized(string json)
        {
            var obj = JsonConvert.DeserializeObject<TypeWithCoinsChangeProperty>(json);

            return obj.Amount.ToString();
        }

        [Test]
        public void Null_can_be_deserialized()
        {
            var obj = JsonConvert.DeserializeObject<TypeWithCoinsChangeProperty>("{\"Amount\":null}");

            Assert.IsNull(obj.Amount);
        }
    }
}

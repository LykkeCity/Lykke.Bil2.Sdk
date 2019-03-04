using System.IO;
using Lykke.Bil2.Contract.Common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class CoinsAmountJsonConverterTests
    {
        private class TypeWithCoinsAmountProperty
        {
            public CoinsAmount Amount { get; set; }
        }

        [Test]
        [TestCase("10.1230", ExpectedResult = "{\"Amount\":\"10.1230\"}")]
        public string Can_be_serialized(string stringValue)
        {
            var obj = new TypeWithCoinsAmountProperty
            {
                Amount = CoinsAmount.Parse(stringValue)
            };

            return JsonConvert.SerializeObject(obj);
        }

        [Test]
        [TestCase("{\"Amount\":\"0.553000\"}", ExpectedResult = "0.553000")]
        public string Can_be_deserialized(string json)
        {
            var obj = JsonConvert.DeserializeObject<TypeWithCoinsAmountProperty>(json);

            return obj.Amount.ToString();
        }

        [Test]
        public void Null_can_be_deserialized()
        {
            var obj = JsonConvert.DeserializeObject<TypeWithCoinsAmountProperty>("{\"Amount\":null}");

            Assert.IsNull(obj.Amount);
        }
    }
}

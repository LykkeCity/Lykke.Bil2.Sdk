using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.JsonConverters;
using Lykke.Numerics.Money;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class UMoneyJsonConverterTests
    {
        private class TypeWithUMoneyProperty
        {
            [JsonConverter(typeof(UMoneyJsonConverter))]
            public UMoney? Amount { get; set; }
        }

        [Test]
        [TestCase("10.1230", ExpectedResult = "{\"Amount\":\"10.1230\"}")]
        public string Can_be_serialized(string stringValue)
        {
            var obj = new TypeWithUMoneyProperty
            {
                Amount = UMoney.Parse(stringValue)
            };

            return JsonConvert.SerializeObject(obj);
        }
        
        [Test]
        public void Null_can_be_serialized()
        {
            var obj = new TypeWithUMoneyProperty
            {
                Amount = null
            };

            var actualResult = JsonConvert.SerializeObject(obj);
            
            Assert.AreEqual("{\"Amount\":null}", actualResult);
        }

        [Test]
        [TestCase("{\"Amount\":\"0.553000\"}", ExpectedResult = "0.553000")]
        public string Can_be_deserialized(string json)
        {
            var obj = JsonConvert.DeserializeObject<TypeWithUMoneyProperty>(json);

            return obj.Amount.ToString();
        }

        [Test]
        public void Null_can_be_deserialized()
        {
            var obj = JsonConvert.DeserializeObject<TypeWithUMoneyProperty>("{\"Amount\":null}");

            Assert.IsNull(obj.Amount);
        }
    }
}

using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.Blockchains.Integrations.Contract.Tests
{
    [TestFixture]
    public class Base64JsonConverterTests
    {
        private class TypeWithBase64Property
        {
            public Base64String Base64Property { get; set; }
        }

        [Test]
        [TestCase(null, ExpectedResult = "{\"Base64Property\":null}")]
        [TestCase("", ExpectedResult = "{\"Base64Property\":\"\"}")]
        [TestCase("the string", ExpectedResult = "{\"Base64Property\":\"dGhlIHN0cmluZw==\"}")]
        public string Can_be_serailized(string stringValue)
        {
            var obj = new TypeWithBase64Property
            {
                Base64Property = stringValue
            };

            return JsonConvert.SerializeObject(obj);
        }

        [Test]
        [TestCase("{\"Base64Property\":null}", ExpectedResult = null)]
        [TestCase("{\"Base64Property\":\"\"}", ExpectedResult = "")]
        [TestCase("{\"Base64Property\":\"dGhlIHN0cmluZw==\"}", ExpectedResult = "the string")]
        public string Can_be_deserailized(string json)
        {
            var obj = JsonConvert.DeserializeObject<TypeWithBase64Property>(json);

            return obj.Base64Property;
        }
    }
}

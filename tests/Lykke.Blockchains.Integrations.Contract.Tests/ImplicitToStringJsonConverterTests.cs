using Lykke.Blockchains.Integrations.Contract.Common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.Blockchains.Integrations.Contract.Tests
{
    [TestFixture]
    public class ImplicitToStringJsonConverterTests
    {
        private class TypeWithAssetIdProperty
        {
            public AssetId AssetId { get; set; }
        }

        [Test]
        [TestCase(null, ExpectedResult = "{\"AssetId\":null}")]
        [TestCase("123", ExpectedResult = "{\"AssetId\":\"123\"}")]
        public string Can_be_serailized(string stringValue)
        {
            var obj = new TypeWithAssetIdProperty
            {
                AssetId = stringValue
            };

            return JsonConvert.SerializeObject(obj);
        }

        [Test]
        [TestCase("{\"AssetId\":null}", ExpectedResult = null)]
        [TestCase("{\"AssetId\":\"123\"}", ExpectedResult = "123")]
        public string Can_be_deserailized(string json)
        {
            var obj = JsonConvert.DeserializeObject<TypeWithAssetIdProperty>(json);

            return obj.AssetId;
        }
    }
}
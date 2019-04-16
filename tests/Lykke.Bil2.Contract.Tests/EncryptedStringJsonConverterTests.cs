using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class EncryptedStringJsonConverterTests
    {
        private const string EncryptedText = "Y64DzytWdvNjuTsSfiLYm42kbcCpE12GwSRiqnZTKZs6pVx737xS53CTaQtu5VWV8mUzrcufiGrripKfyowVU5VdNu3yXBKK5Sa5V3PNdKxLvgQrr9Z9VQdfvHJjVKcSURpJH9Mjd7kUzmiJYNcTnwV5oQUmcmtUo6JaQHq755BHeQkAWEBzoL99LijcYMsWCs38EkNrBb31noPc7aqa6Y9VGKmPteYxpUS8oqdgETGGP6hXJby14uxMnB6abRwuk5EcyoxVQTLPxVhFENVDYZ1rGe1VKvHYo4fs3MMt4ufqYtrM4hQVqXsnTvRQz6aM9Dckt";

        private class TypeWithEncryptedStringProperty
        {
            public EncryptedString EncryptedString { get; set; }
        }
        
        [Test]
        [TestCase(null, ExpectedResult = "{\"EncryptedString\":null}")]
        [TestCase(EncryptedText, ExpectedResult = "{\"EncryptedString\":\"Y64DzytWdvNjuTsSfiLYm42kbcCpE12GwSRiqnZTKZs6pVx737xS53CTaQtu5VWV8mUzrcufiGrripKfyowVU5VdNu3yXBKK5Sa5V3PNdKxLvgQrr9Z9VQdfvHJjVKcSURpJH9Mjd7kUzmiJYNcTnwV5oQUmcmtUo6JaQHq755BHeQkAWEBzoL99LijcYMsWCs38EkNrBb31noPc7aqa6Y9VGKmPteYxpUS8oqdgETGGP6hXJby14uxMnB6abRwuk5EcyoxVQTLPxVhFENVDYZ1rGe1VKvHYo4fs3MMt4ufqYtrM4hQVqXsnTvRQz6aM9Dckt\"}")]
        public string Can_be_serailized(string encryptedString)
        {
            var obj = new TypeWithEncryptedStringProperty
            {
                EncryptedString = encryptedString != null ? new EncryptedString(new Base58String(encryptedString)) : null
            };

            return JsonConvert.SerializeObject(obj);
        }

        [Test]
        [TestCase("{\"EncryptedString\":null}", ExpectedResult = null)]
        [TestCase("{\"EncryptedString\":\"Y64DzytWdvNjuTsSfiLYm42kbcCpE12GwSRiqnZTKZs6pVx737xS53CTaQtu5VWV8mUzrcufiGrripKfyowVU5VdNu3yXBKK5Sa5V3PNdKxLvgQrr9Z9VQdfvHJjVKcSURpJH9Mjd7kUzmiJYNcTnwV5oQUmcmtUo6JaQHq755BHeQkAWEBzoL99LijcYMsWCs38EkNrBb31noPc7aqa6Y9VGKmPteYxpUS8oqdgETGGP6hXJby14uxMnB6abRwuk5EcyoxVQTLPxVhFENVDYZ1rGe1VKvHYo4fs3MMt4ufqYtrM4hQVqXsnTvRQz6aM9Dckt\"}", ExpectedResult = EncryptedText)]
        public string Can_be_deserailized(string json)
        {
            var obj = JsonConvert.DeserializeObject<TypeWithEncryptedStringProperty>(json);

            return obj.EncryptedString?.EncryptedValue?.Value;
        }
    }
}

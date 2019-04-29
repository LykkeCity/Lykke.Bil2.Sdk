using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class EncryptedStringJsonConverterTests
    {
        private const string EncryptedText = "eyJlbmNyeXB0ZWRBZXNLZXlzIjoiZllJeHJCcGhjNGMvVEVlbUJwbVV5Mis5NDd3THVTWGQ5Yk9lZHVrbThPWDNOVVhZMVJVZUpJTnJRM1REM1Y1UTRheWsvNnF6VzNoVE52S0g5U1Bad3JIcUhqWXJ5MVFnQ1pNL3F4RVBIWFp4eDNiUzE5TXU3cVVnSFJ2MUQ0eG9DcW1UK3lxakNpTTlnWDVsZFFocnc2UEVuaUdodVlBMWcra3lrS2xFcFpvPSIsImVuY3J5cHRlZEJvZHkiOiJwUytKenk4L2djdFVHcmlyMWpmaTVBPT0ifQ==";

        private class TypeWithEncryptedStringProperty
        {
            public EncryptedString EncryptedString { get; set; }
        }
        
        [Test]
        [TestCase(null, ExpectedResult = "{\"EncryptedString\":null}")]
        [TestCase(EncryptedText, ExpectedResult = "{\"EncryptedString\":\"eyJlbmNyeXB0ZWRBZXNLZXlzIjoiZllJeHJCcGhjNGMvVEVlbUJwbVV5Mis5NDd3THVTWGQ5Yk9lZHVrbThPWDNOVVhZMVJVZUpJTnJRM1REM1Y1UTRheWsvNnF6VzNoVE52S0g5U1Bad3JIcUhqWXJ5MVFnQ1pNL3F4RVBIWFp4eDNiUzE5TXU3cVVnSFJ2MUQ0eG9DcW1UK3lxakNpTTlnWDVsZFFocnc2UEVuaUdodVlBMWcra3lrS2xFcFpvPSIsImVuY3J5cHRlZEJvZHkiOiJwUytKenk4L2djdFVHcmlyMWpmaTVBPT0ifQ==\"}")]
        public string Can_be_serailized(string encryptedString)
        {
            var obj = new TypeWithEncryptedStringProperty
            {
                EncryptedString = encryptedString != null ? new EncryptedString(new Base64String(encryptedString)) : null
            };

            return JsonConvert.SerializeObject(obj);
        }

        [Test]
        [TestCase("{\"EncryptedString\":null}", ExpectedResult = null)]
        [TestCase("{\"EncryptedString\":\"eyJlbmNyeXB0ZWRBZXNLZXlzIjoiZllJeHJCcGhjNGMvVEVlbUJwbVV5Mis5NDd3THVTWGQ5Yk9lZHVrbThPWDNOVVhZMVJVZUpJTnJRM1REM1Y1UTRheWsvNnF6VzNoVE52S0g5U1Bad3JIcUhqWXJ5MVFnQ1pNL3F4RVBIWFp4eDNiUzE5TXU3cVVnSFJ2MUQ0eG9DcW1UK3lxakNpTTlnWDVsZFFocnc2UEVuaUdodVlBMWcra3lrS2xFcFpvPSIsImVuY3J5cHRlZEJvZHkiOiJwUytKenk4L2djdFVHcmlyMWpmaTVBPT0ifQ==\"}", ExpectedResult = EncryptedText)]
        public string Can_be_deserailized(string json)
        {
            var obj = JsonConvert.DeserializeObject<TypeWithEncryptedStringProperty>(json);

            return obj.EncryptedString?.EncryptedValue?.Value;
        }
    }
}

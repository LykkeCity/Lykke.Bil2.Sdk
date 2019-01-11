using System;
using Lykke.Blockchains.Integrations.Contract.Common;
using NUnit.Framework;

namespace Lykke.Blockchains.Integrations.Contract.Tests
{
    [TestFixture]
    public class Base64StringTests
    {
        [Test]
        [TestCase("dGhlIHN0cmluZw==")]
        [TestCase("dGhlIHN0cmluZzE=")]
        [TestCase("dGhlIHN0cmluZzEy")]
        public void Can_create_from_valid_string(string base64Value)
        {
            var value = Base64String.Create(base64Value);

            Assert.AreEqual(base64Value, value.Base64Value);
        }

        [Test]
        public void Cant_create_from_null_string()
        {
            Assert.Throws<ArgumentNullException>(() => Base64String.Create(null));
        }

        [Test]
        [TestCase("not a base64 string")]
        public void Cant_create_from_invalid_string(string base64Value)
        {
            Assert.Throws<Base64StringConversionException>(() => Base64String.Create(base64Value));
        }

        [Test]
        [TestCase("", ExpectedResult = "")]
        [TestCase("the string", ExpectedResult = "dGhlIHN0cmluZw==")]
        public string Can_implicitly_encode_string(string stringValue)
        {
            Base64String value = stringValue;

            return value.Base64Value;
        }

        [Test]
        public void Can_implicitly_pass_through_null_string()
        {
            Base64String value = (string)null;

            Assert.IsNull(value);
        }

        [Test]
        [TestCase(new byte[0], ExpectedResult = "")]
        [TestCase(new byte[]{ 0x00, 0x50, 0xAF, 0xFF, 0x45 }, ExpectedResult = "AFCv/0U=")]
        public string Can_implicitly_encode_bytes(byte[] bytesValue)
        {
            Base64String value = bytesValue;

            return value.Base64Value;
        }

        [Test]
        public void Can_implicitly_pass_through_null_bytes()
        {
            Base64String value = (byte[])null;

            Assert.IsNull(value);
        }

        [Test]
        [TestCase("", ExpectedResult = "")]
        [TestCase("dGhlIHN0cmluZw==", ExpectedResult = "the string")]
        public string Can_implicitly_decode_to_string(string base64Value)
        {
            var value = Base64String.Create(base64Value);

            return value;
        }

        [Test]
        public void Can_implicitly_pass_through_null_to_string()
        {
            string stringValue = (Base64String) null;

            Assert.IsNull(stringValue);
        }

        [Test]
        [TestCase("", ExpectedResult = new byte[0])]
        [TestCase("AFCv/0U=", ExpectedResult = new byte[]{ 0x00, 0x50, 0xAF, 0xFF, 0x45 })]
        public byte[] Can_implicitly_decode_to_bytes(string base64Value)
        {
            var value = Base64String.Create(base64Value);

            return value;
        }

        [Test]
        public void Can_implicitly_pass_through_null_to_bytes()
        {
            byte[] bytes = (Base64String)null;

            Assert.IsNull(bytes);
        }
    }
}

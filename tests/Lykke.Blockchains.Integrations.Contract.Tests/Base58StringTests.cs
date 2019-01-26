using System;
using Lykke.Blockchains.Integrations.Contract.Common;
using NUnit.Framework;

namespace Lykke.Blockchains.Integrations.Contract.Tests
{
    [TestFixture]
    public class Base58StringTests
    {
        [Test]
        [TestCase("5CwmCsCEJzoWSrM1c5bDQrcsHYF3SmfxDr5JAC167r4v6SgX8jtvtjH8WPgbUhqKWDqtsDbbpyEJUNhduXDTrRYCqUDcBNR8Wbg6gY2f4ucxvy3")]
        [TestCase("7YKZiHCxdLJS6i")]
        public void Can_create_from_valid_string(string base58Value)
        {
            var value = Base58String.Create(base58Value);

            Assert.AreEqual(base58Value, value.Value);
        }

        [Test]
        public void Cant_create_from_null_string()
        {
            Assert.Throws<ArgumentNullException>(() => Base58String.Create(null));
        }

        [Test]
        [TestCase("not a base58 string")]
        public void Cant_create_from_invalid_string(string base58Value)
        {
            Assert.Throws<Base58StringConversionException>(() => Base58String.Create(base58Value));
        }

        [Test]
        [TestCase("", ExpectedResult = "")]
        [TestCase("the string", ExpectedResult = "7YKZiHCxdLJS6i")]
        public string Can_implicitly_encode_string(string stringValue)
        {
            Base58String value = stringValue;

            return value.Value;
        }

        [Test]
        public void Can_implicitly_pass_through_null_string()
        {
            Base58String value = (string)null;

            Assert.IsNull(value);
        }

        [Test]
        [TestCase(new byte[0], ExpectedResult = "")]
        [TestCase(new byte[]{ 0x00, 0x50, 0xAF, 0xFF, 0x45 }, ExpectedResult = "134d8FE")]
        public string Can_implicitly_encode_bytes(byte[] bytesValue)
        {
            Base58String value = bytesValue;

            return value.Value;
        }

        [Test]
        public void Can_implicitly_pass_through_null_bytes()
        {
            Base58String value = (byte[])null;

            Assert.IsNull(value);
        }

        [Test]
        [TestCase("", ExpectedResult = "")]
        [TestCase("7YKZiHCxdLJS6i", ExpectedResult = "the string")]
        public string Can_implicitly_decode_to_string(string base58Value)
        {
            var value = Base58String.Create(base58Value);

            return value;
        }

        [Test]
        public void Can_implicitly_pass_through_null_to_string()
        {
            string stringValue = (Base58String) null;

            Assert.IsNull(stringValue);
        }

        [Test]
        [TestCase("", ExpectedResult = new byte[0])]
        [TestCase("134d8FE", ExpectedResult = new byte[]{ 0x00, 0x50, 0xAF, 0xFF, 0x45 })]
        public byte[] Can_implicitly_decode_to_bytes(string base58Value)
        {
            var value = Base58String.Create(base58Value);

            return value;
        }

        [Test]
        public void Can_implicitly_pass_through_null_to_bytes()
        {
            byte[] bytes = (Base58String)null;

            Assert.IsNull(bytes);
        }
    }
}

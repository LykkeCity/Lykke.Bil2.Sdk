﻿using System;
using Lykke.Bil2.Contract.Common;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class Base58StringTests
    {
        [Test]
        [TestCase("5CwmCsCEJzoWSrM1c5bDQrcsHYF3SmfxDr5JAC167r4v6SgX8jtvtjH8WPgbUhqKWDqtsDbbpyEJUNhduXDTrRYCqUDcBNR8Wbg6gY2f4ucxvy3")]
        [TestCase("7YKZiHCxdLJS6i")]
        public void Can_create_from_valid_string(string base58Value)
        {
            var value = new Base58String(base58Value);

            Assert.AreEqual(base58Value, value.Value);
        }

        [Test]
        public void Cant_create_from_null_string()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Base58String(null));
        }

        [Test]
        [TestCase("not a base58 string")]
        public void Cant_create_from_invalid_string(string base58Value)
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<Base58StringConversionException>(() => new Base58String(base58Value));
        }

        [Test]
        [TestCase("", ExpectedResult = "")]
        [TestCase("the string", ExpectedResult = "7YKZiHCxdLJS6i")]
        public string Can_encode_string(string stringValue)
        {
            var value = Base58String.Encode(stringValue);

            return value.Value;
        }

        [Test]
        [TestCase(new byte[0], ExpectedResult = "")]
        [TestCase(new byte[]{ 0x00, 0x50, 0xAF, 0xFF, 0x45 }, ExpectedResult = "134d8FE")]
        public string Can_encode_bytes(byte[] bytesValue)
        {
            var value = Base58String.Encode(bytesValue);

            return value.Value;
        }

        [Test]
        [TestCase("", ExpectedResult = "")]
        [TestCase("7YKZiHCxdLJS6i", ExpectedResult = "the string")]
        public string Can_decode_to_string(string base58Value)
        {
            var value = new Base58String(base58Value);

            return value.DecodeToString();
        }

        [Test]
        [TestCase("", ExpectedResult = new byte[0])]
        [TestCase("134d8FE", ExpectedResult = new byte[]{ 0x00, 0x50, 0xAF, 0xFF, 0x45 })]
        public byte[] Can_decode_to_bytes(string base58Value)
        {
            var value = new Base58String(base58Value);

            return value.DecodeToBytes();
        }
    }
}
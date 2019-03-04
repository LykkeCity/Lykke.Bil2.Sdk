using System;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Numerics.Money;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class CoinsAmountTests
    {
        [Test]
        [TestCase("0")]
        [TestCase("123")]
        [TestCase("0.000")]
        [TestCase("0.0012300")]
        [TestCase("123.00000")]
        [TestCase("123.56789")]
        public void Can_create_from_valid_coins_string(string stringValue)
        {
            var value = CoinsAmount.Parse(stringValue);

            Assert.AreEqual(stringValue, value.ToString());
        }

        [Test]
        public void Cant_create_from_null_string()
        {
            Assert.Throws<ArgumentNullException>(() => CoinsAmount.Parse(null));
        }

        [Test]
        [TestCase("")]
        [TestCase("-1")]
        [TestCase(" 1")]
        [TestCase("1 ")]
        [TestCase("(1)")]
        [TestCase("12,123.00000")]
        [TestCase("12123,00000")]
        [TestCase("123.00.000")]
        [TestCase("1E10")]
        [TestCase("ABC")]
        public void Cant_create_from_invalid_coins_string(string stringValue)
        {
            Assert.Throws<CoinsConversionException>(() => CoinsAmount.Parse(stringValue));
        }

        [Test]
        [TestCase(0, 0, ExpectedResult = "0")]
        [TestCase(0, 5, ExpectedResult = "0.00000")]
        [TestCase(1, 0, ExpectedResult = "1")]
        [TestCase(1, 1, ExpectedResult = "1.0")]
        [TestCase(100, 0, ExpectedResult = "100")]
        [TestCase(100, 8, ExpectedResult = "100.00000000")]
        [TestCase(100.123456, 3, ExpectedResult = "100.123")]
        [TestCase(0.123456, 4, ExpectedResult = "0.1235")]
        public string Can_convert_from_decimal(decimal decimalValue, int accuracy)
        {
            var value = Money.Create(decimalValue, accuracy).ToCoinsAmount();

            return value.ToString();
        }

        [Test]
        public void Cant_convert_from_negative_decimal(
            [Values(-1, -15, -0.003)] decimal decimalValue, 
            [Values(0, 1, 5)] int accuracy)
        {
            var money = Money.Create(decimalValue, accuracy);

            if (money != Money.Zero)
            {
                Assert.Throws<CoinsConversionException>(() => money.ToCoinsAmount());
            }
        }

        [Test]
        public void Can_implicitly_convert_to_money()
        {
            Money value = CoinsAmount.Parse("0");
        }
    }
}

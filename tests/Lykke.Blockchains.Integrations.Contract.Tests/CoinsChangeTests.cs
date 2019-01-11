using System;
using Lykke.Blockchains.Integrations.Contract.Common;
using NUnit.Framework;

namespace Lykke.Blockchains.Integrations.Contract.Tests
{
    [TestFixture]
    public class CoinsChangeTests
    {
        [Test]
        [TestCase("0")]
        [TestCase("123")]
        [TestCase("0.000")]
        [TestCase("0.0012300")]
        [TestCase("123.00000")]
        [TestCase("123.56789")]
        [TestCase("-1")]
        [TestCase("-400")]
        [TestCase("-0.12300")]
        [TestCase("-100.12300")]
        [TestCase("+1")]
        [TestCase("+400")]
        [TestCase("+0.12300")]
        [TestCase("+100.12300")]
        public void Can_create_from_valid_coins_string(string stringValue)
        {
            var value = CoinsChange.Create(stringValue);

            Assert.AreEqual(stringValue, value.StringValue);
        }

        [Test]
        public void Cant_create_from_null_string()
        {
            Assert.Throws<ArgumentNullException>(() => CoinsChange.Create(null));
        }

        [Test]
        [TestCase("")]
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
            Assert.Throws<CoinsConversionException>(() => CoinsChange.Create(stringValue));
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
        [TestCase(-1, 0, ExpectedResult = "-1")]
        [TestCase(-1, 1, ExpectedResult = "-1.0")]
        [TestCase(-100, 0, ExpectedResult = "-100")]
        [TestCase(-100, 8, ExpectedResult = "-100.00000000")]
        [TestCase(-100.123456, 3, ExpectedResult = "-100.123")]
        [TestCase(-0.123456, 4, ExpectedResult = "-0.1235")]
        public string Can_convert_from_decimal(decimal decimalValue, int accuracy)
        {
            var value = CoinsChange.FromDecimal(decimalValue, accuracy);

            return value.StringValue;
        }

        [Test]
        [TestCase("0", ExpectedResult = 0)]
        [TestCase("0.00000", ExpectedResult = 0)]
        [TestCase("1", ExpectedResult = 1)]
        [TestCase("1.0", ExpectedResult = 1)]
        [TestCase("100", ExpectedResult = 100)]
        [TestCase("100.00000000", ExpectedResult = 100)]
        [TestCase("100.123", ExpectedResult = 100.123)]
        [TestCase("0.1235", ExpectedResult = 0.1235)]
        [TestCase("-1", ExpectedResult = -1)]
        [TestCase("-1.0", ExpectedResult = -1)]
        [TestCase("-100", ExpectedResult = -100)]
        [TestCase("-100.00000000", ExpectedResult = -100)]
        [TestCase("-100.123", ExpectedResult = -100.123)]
        [TestCase("-0.1235", ExpectedResult = -0.1235)]
        public decimal Can_implicitly_convert_to_decimal(string stringValue)
        {
            return CoinsChange.Create(stringValue);
        }

        [Test]
        [TestCase(0, -1)]
        [TestCase(4, 29)]
        public void Accuracy_should_be_in_valid_range(
            [Values(0, 1, 4)] decimal decimalValue, 
            [Values(-10, -1, 29, 40)] int accuracy)
        {
            Assert.Throws<CoinsConversionException>(() => CoinsChange.FromDecimal(decimalValue, accuracy));
        }
    }
}

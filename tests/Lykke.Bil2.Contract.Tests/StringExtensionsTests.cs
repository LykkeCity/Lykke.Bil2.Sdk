using Lykke.Bil2.Contract.Common.Extensions;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        [TestCase("X", ExpectedResult = "x")]
        [TestCase("XXaa", ExpectedResult = "x-xaa")]
        [TestCase("XXXaa", ExpectedResult = "x-x-xaa")]
        [TestCase("Eos", ExpectedResult = "eos")]
        [TestCase("EthereumClassic", ExpectedResult = "ethereum-classic")]
        [TestCase("VeryLongBlockchainName", ExpectedResult = "very-long-blockchain-name")]
        public string Test_kebab(string integrationName)
        {
            return integrationName.CamelToKebab();
        }
    }
}

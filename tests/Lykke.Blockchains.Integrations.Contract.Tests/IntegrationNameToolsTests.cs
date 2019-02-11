using Lykke.Blockchains.Integrations.Contract.Common;
using NUnit.Framework;

namespace Lykke.Blockchains.Integrations.Contract.Tests
{
    [TestFixture]
    public class IntegrationNameToolsTests
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
            return IntegrationNameTools.ToKebab(integrationName);
        }
    }
}

using Lykke.Blockchains.Integrations.Contract.Common;
using NUnit.Framework;

namespace Lykke.Blockchains.Integrations.Contract.Tests
{
    [TestFixture]
    public class IntegrationNameToolsTests
    {
        [Test]
        [TestCase("Eos", ExpectedResult = "Eos")]
        [TestCase("Ethereum Classic", ExpectedResult = "EthereumClassic")]
        [TestCase("Very Long Blockchain Name", ExpectedResult = "VeryLongBlockchainName")]
        public string Test_came_case(string integrationName)
        {
            return IntegrationNameTools.ToCamelCase(integrationName);
        }

        [Test]
        [TestCase("Eos", ExpectedResult = "eos")]
        [TestCase("Ethereum Classic", ExpectedResult = "ethereum-classic")]
        [TestCase("Very Long Blockchain Name", ExpectedResult = "very-long-blockchain-name")]
        public string Test_kebab(string integrationName)
        {
            return IntegrationNameTools.ToKebab(integrationName);
        }
    }
}

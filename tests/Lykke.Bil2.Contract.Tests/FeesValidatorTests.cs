using System;
using System.Linq;
using Lykke.Bil2.Contract.Common;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class FeesValidatorTests
    {
        [Test]
        [TestCase("KIN", false)]
        [TestCase("KIN,STEEM", false)]
        [TestCase("KIN,STEEM,XRP", false)]
        [TestCase("KIN,STEEM,KIN,XRP", true)]
        [TestCase("KIN,KIN,KIN", true)]
        [TestCase("XRP,KIN,XRP", true)]
        public void Test_that_asset_duplications_are_not_allowed(string assets, bool shouldThrow)
        {
            var fees = assets.Split(',')
                .Select(a => new Fee(new Asset(a), 10))
                .ToArray();
            
            if (shouldThrow)
            {
                Assert.Throws<ArgumentException>(() => FeesValidator.ValidateFeesInResponse(fees));
            }
            else
            {
                FeesValidator.ValidateFeesInResponse(fees);
            }
        }
    }
}

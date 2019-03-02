using System;
using System.Linq;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class TransactionCoinsValidatorTests
    {
        [Test]
        public void Test_that_empty_collections_are_not_allowed()
        {
            // Arrange

            var coinToSpend = new CoinToSpend
            (
                "1",
                1,
                "KIN",
                CoinsAmount.FromDecimal(100, 3),
                "A"
            );
            var coinToReceive = new CoinToReceive
            (
                "KIN",
                CoinsAmount.FromDecimal(100, 3),
                "A"
            );

            // Act, Throw

            Assert.Throws<RequestValidationException>
            (
                () => TransactionCoinsValidator.Validate(null, new[] { coinToReceive })
            );

            Assert.Throws<RequestValidationException>
            (
                () => TransactionCoinsValidator.Validate(Array.Empty<CoinToSpend>(), new[] { coinToReceive })
            );

            Assert.Throws<RequestValidationException>
            (
                () => TransactionCoinsValidator.Validate(new[] { coinToSpend }, null )
            );

            Assert.Throws<RequestValidationException>
            (
                () => TransactionCoinsValidator.Validate(new[] { coinToSpend }, Array.Empty<CoinToReceive>() )
            );
        }

        [Test]
        [TestCase("KIN", "STEEM", true)]
        [TestCase("KIN,STEEM", "STEEM", true)]
        [TestCase("KIN,STEEM", "KIN", true)]
        [TestCase("STEEM", "KIN,STEEM", true)]
        [TestCase("KIN", "KIN,STEEM", true)]
        [TestCase("KIN,STEEM,RSK", "KIN,STEEM", true)]
        [TestCase("KIN,STEEM,RSK", "STEEM,RSK", true)]
        [TestCase("KIN", "KIN", false)]
        [TestCase("KIN,STEEM", "KIN,STEEM", false)]
        [TestCase("KIN,STEEM", "STEEM,KIN", false)]
        [TestCase("KIN,STEEM,RSK", "STEEM,RSK,KIN", false)]
        [TestCase("KIN,STEEM,RSK", "RSK,STEEM,KIN", false)]
        public void Test_that_sets_of_assets_should_match(string assetsToSpend, string assetsToReceive, bool shouldThrow)
        {
            // Arrange

            var coinsToSpend = assetsToSpend
                .Split(',')
                .Select(asset => new CoinToSpend
                (
                    "1",
                    1,
                    asset,
                    CoinsAmount.FromDecimal(100, 3),
                    "A"
                ))
                .ToArray();
            var coinsToReceive = assetsToReceive
                .Split(',')
                .Select(asset => new CoinToReceive
                (
                    asset,
                    CoinsAmount.FromDecimal(100, 3),
                    "A"
                ))
                .ToArray();

            // Act, Throw

            if (shouldThrow)
            {
                Assert.Throws<RequestValidationException>(() => TransactionCoinsValidator.Validate(coinsToSpend, coinsToReceive));
            }
            else
            {
                Assert.DoesNotThrow(() => TransactionCoinsValidator.Validate(coinsToSpend, coinsToReceive));
            }
        }

        [Test]
        [TestCase("KIN:100","KIN:100", false)]
        [TestCase("KIN:100,KIN:200","KIN:300", false)]
        [TestCase("KIN:100,KIN:200","KIN:150,KIN:150", false)]
        [TestCase("KIN:100,KIN:200,STEEM:100","KIN:150,KIN:150,STEEM:70,STEEM:30", false)]
        [TestCase("KIN:100,KIN:200,STEEM:100,RSK:50","KIN:150,KIN:150,STEEM:70,STEEM:30,RSK:50", false)]
        [TestCase("KIN:100,KIN:200,STEEM:100,RSK:50","KIN:150,KIN:160,STEEM:70,STEEM:30,RSK:40", true)]
        [TestCase("KIN:100,KIN:200,STEEM:100,RSK:50","KIN:300", true)]
        [TestCase("KIN:100,KIN:200,STEEM:100,RSK:50","KIN:450", true)]
        [TestCase("KIN:100,KIN:200","KIN:400", true)]
        [TestCase("KIN:100,KIN:200","KIN:200", true)]
        public void Test_that_sum_of_coins_to_send_and_to_receive_matches_for_each_assets(string assetsToSpend, string assetsToReceive, bool shouldThrow)
        {
            // Arrange

            var coinsToSpend = assetsToSpend
                .Split(',')
                .Select(x => x.Split(':'))
                .Select(x => new CoinToSpend
                (
                    "1",
                    1,
                    x[0],
                    CoinsAmount.FromDecimal(int.Parse(x[1]), 3),
                    "A"
                ))
                .ToArray();
            var coinsToReceive = assetsToReceive
                .Split(',')
                .Select(x => x.Split(':'))
                .Select(x => new CoinToReceive
                (
                    x[0],
                    CoinsAmount.FromDecimal(int.Parse(x[1]), 3),
                    "A"
                ))
                .ToArray();

            // Act, Throw

            if (shouldThrow)
            {
                Assert.Throws<RequestValidationException>(() => TransactionCoinsValidator.Validate(coinsToSpend, coinsToReceive));
            }
            else
            {
                Assert.DoesNotThrow(() => TransactionCoinsValidator.Validate(coinsToSpend, coinsToReceive));
            }
        }
    }
}

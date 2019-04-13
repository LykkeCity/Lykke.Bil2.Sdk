using System;
using System.Linq;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;
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
                new CoinId("1", 1),
                new Asset("KIN"),
                UMoney.Create(100, 3),
                "A"
            );
            var coinToReceive = new CoinToReceive
            (
                0,
                new Asset("KIN"),
                UMoney.Create(100, 3),
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
                    new CoinId("1", 1),
                    new Asset(asset),
                    UMoney.Create(100, 3),
                    "A"
                ))
                .ToArray();
            var coinsToReceive = assetsToReceive
                .Split(',')
                .Select((asset, i) => new CoinToReceive
                (
                    i,
                    new Asset(asset),
                    UMoney.Create(100, 3),
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
        [TestCase("KIN:100","KIN:90", false)]
        [TestCase("KIN:100","KIN:110", true)]
        [TestCase("KIN:100,KIN:200","KIN:300", false)]
        [TestCase("KIN:100,KIN:200","KIN:290", false)]
        [TestCase("KIN:100,KIN:200","KIN:310", true)]
        [TestCase("KIN:100,KIN:200","KIN:150,KIN:150", false)]
        [TestCase("KIN:100,KIN:200","KIN:150,KIN:140", false)]
        [TestCase("KIN:100,KIN:200","KIN:150,KIN:160", true)]
        [TestCase("KIN:100,KIN:200,STEEM:100","KIN:150,KIN:150,STEEM:70,STEEM:30", false)]
        [TestCase("KIN:100,KIN:200,STEEM:100","KIN:150,KIN:150,STEEM:70,STEEM:20", false)]
        [TestCase("KIN:100,KIN:200,STEEM:100","KIN:150,KIN:150,STEEM:70,STEEM:40", true)]
        [TestCase("KIN:100,KIN:200,STEEM:100,RSK:50","KIN:150,KIN:150,STEEM:70,STEEM:30,RSK:50", false)]
        [TestCase("KIN:100,KIN:200,STEEM:100,RSK:50","KIN:150,KIN:150,STEEM:70,STEEM:30,RSK:40", false)]
        [TestCase("KIN:100,KIN:200,STEEM:100,RSK:50","KIN:150,KIN:150,STEEM:70,STEEM:30,RSK:60", true)]
        [TestCase("KIN:100,KIN:200,STEEM:100,RSK:50","KIN:300", true)]
        [TestCase("KIN:100,KIN:200,STEEM:100,RSK:50","KIN:450", true)]
        public void Test_that_sum_of_coins_to_send_equal_or_greater_than_sum_to_receive_for_each_assets(string assetsToSpend, string assetsToReceive, bool shouldThrow)
        {
            // Arrange

            var coinsToSpend = assetsToSpend
                .Split(',')
                .Select(x => x.Split(':'))
                .Select(x => new CoinToSpend
                (
                    new CoinId("1", 1),
                    new Asset(x[0]),
                    UMoney.Create(int.Parse(x[1]), 3),
                    "A"
                ))
                .ToArray();
            var coinsToReceive = assetsToReceive
                .Split(',')
                .Select(x => x.Split(':'))
                .Select((x, i) => new CoinToReceive
                (
                    i,
                    new Asset(x[0]),
                    UMoney.Create(int.Parse(x[1]), 3),
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
        [TestCase("0", false)]
        [TestCase("0,1,2,3", false)]
        [TestCase("0,3,1,2", false)]
        [TestCase("1", true)]
        [TestCase("3", true)]
        [TestCase("1,2", true)]
        [TestCase("0,1,2,4", true)]
        [TestCase("0,2,4,1", true)]
        public void Test_that_coins_to_receive_should_be_numbers_in_a_row_starting_from_zero(string coinsToReceiveNumbers, bool shouldThrow)
        {
            var coinToSpend = new CoinToSpend
            (
                new CoinId("1", 1),
                new Asset("KIN"),
                UMoney.Create(100, 3),
                "A"
            );
            var coinsToReceive = coinsToReceiveNumbers
                .Split(',')
                .Select(x => new CoinToReceive
                (
                    int.Parse(x),
                    new Asset("KIN"),
                    UMoney.Create(1, 3),
                    "B"
                ))
                .ToArray();

            if (shouldThrow)
            {
                Assert.Throws<RequestValidationException>(() => TransactionCoinsValidator.Validate(new[] {coinToSpend}, coinsToReceive));
            }
            else
            {
                Assert.DoesNotThrow(() => TransactionCoinsValidator.Validate(new[] {coinToSpend}, coinsToReceive));
            }
        }
    }
}

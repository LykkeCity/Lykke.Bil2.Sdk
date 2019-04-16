using System;
using System.Collections.Generic;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class TransactionTransfersValidatorTests
    {
        [Test]
        public void Test_that_single_transfer_is_allowed()
        {
            TransactionTransfersValidator.Validate(new List<Transfer>
            {
                new Transfer
                ( 
                    new Asset("STEEM"),
                    UMoney.Create(100, 3),
                    "A",
                    "B"
                )
            });
        }

        [Test]
        [TestCase("STEEM", "KIN", "EOS", "A", "B", "A", "B", "A", "B")]
        [TestCase("KIN", "KIN", "KIN", "A", "B", "C", "D", "E", "F")]
        [TestCase("KIN", "KIN", "KIN", "A", "B", "A", "C", "A", "D")]
        [TestCase("KIN", "KIN", "KIN", "A", "B", "C", "B", "D", "B")]
        public void Test_that_multiple_transfers_are_allowed(
            string asset1, 
            string asset2, 
            string asset3, 
            string source1, 
            string source2, 
            string source3, 
            string destination1, 
            string destination2,
            string destination3)
        {
            TransactionTransfersValidator.Validate(new List<Transfer>
            {
                new Transfer
                ( 
                    new Asset(asset1),
                    UMoney.Create(100, 3),
                    source1,
                    destination1
                ),
                new Transfer
                ( 
                    new Asset(asset2),
                    UMoney.Create(80, 5),
                    source2,
                    destination2
                ),
                new Transfer
                ( 
                    new Asset(asset3),
                    UMoney.Create(80, 5),
                    source3,
                    destination3
                )
            });
        }

        [Test]
        public void Test_that_null_is_not_allowed()
        {
            Assert.Throws<RequestValidationException>(() => TransactionTransfersValidator.Validate(null));
        }

        [Test]
        public void Test_that_empty_collection_is_not_allowed()
        {
            Assert.Throws<RequestValidationException>(() => TransactionTransfersValidator.Validate(Array.Empty<Transfer>()));
        }

        [Test]
        public void Test_that_duplicated_transfers_are_not_allowed()
        {
            var transfers = new List<Transfer>
            {
                new Transfer
                (
                    new Asset("XRP"),
                    UMoney.Create(100, 3),
                    "A",
                    "B"
                ),
                new Transfer
                (
                    new Asset("XRP"),
                    UMoney.Create(80, 5),
                    "A",
                    "B"
                )
            };

            Assert.Throws<RequestValidationException>(() => TransactionTransfersValidator.Validate(transfers));
        }
    }
}

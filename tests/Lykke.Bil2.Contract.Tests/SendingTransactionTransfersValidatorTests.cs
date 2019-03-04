using System;
using System.Collections.Generic;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Numerics.Money;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class SendingTransactionTransfersValidatorTests
    {
        [Test]
        public void Test_that_single_transfer_is_allowed()
        {
            SendingTransactionTransfersValidator.Validate(new List<Transfer>
            {
                new Transfer
                ( 
                    "STEEM",
                    Money.Create(100, 3).ToCoinsAmount(),
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
            SendingTransactionTransfersValidator.Validate(new List<Transfer>
            {
                new Transfer
                ( 
                    asset1,
                    Money.Create(100, 3).ToCoinsAmount(),
                    source1,
                    destination1
                ),
                new Transfer
                ( 
                    asset2,
                    Money.Create(80, 5).ToCoinsAmount(),
                    source2,
                    destination2
                ),
                new Transfer
                ( 
                    asset3,
                    Money.Create(80, 5).ToCoinsAmount(),
                    source3,
                    destination3
                )
            });
        }

        [Test]
        public void Test_that_null_is_not_allowed()
        {
            Assert.Throws<RequestValidationException>(() => SendingTransactionTransfersValidator.Validate(null));
        }

        [Test]
        public void Test_that_empty_collection_is_not_allowed()
        {
            Assert.Throws<RequestValidationException>(() => SendingTransactionTransfersValidator.Validate(Array.Empty<Transfer>()));
        }

        [Test]
        public void Test_that_duplicated_transfers_are_not_allowed()
        {
            var transfers = new List<Transfer>
            {
                new Transfer
                (
                    "XRP",
                    Money.Create(100, 3).ToCoinsAmount(),
                    "A",
                    "B"
                ),
                new Transfer
                (
                    "XRP",
                    Money.Create(80, 5).ToCoinsAmount(),
                    "A",
                    "B"
                )
            };

            Assert.Throws<RequestValidationException>(() => SendingTransactionTransfersValidator.Validate(transfers));
        }
    }
}

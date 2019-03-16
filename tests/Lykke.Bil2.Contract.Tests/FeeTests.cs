using Lykke.Bil2.Contract.Common;
using Lykke.Numerics;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class FeeTests
    {
        [Test]
        [TestCase("KIN:10.0", "KIN:10.0", ExpectedResult = true)]
        [TestCase("KIN:10.0", "KIN:100.0", ExpectedResult = false)]
        [TestCase("KIN:10.0", "STEEM:10.0", ExpectedResult = false)]
        [TestCase("KIN:10.0", "STEEM:100.0", ExpectedResult = false)]
        public bool Test_equation(string a, string b)
        {
            var aParts = a.Split(':');
            var bParts = b.Split(':');

            var aFee = new Fee(new Asset(aParts[0]), UMoney.Parse(aParts[1]));
            var bFee = new Fee(new Asset(bParts[0]), UMoney.Parse(bParts[1]));

            return Equals(aFee, bFee);
        }

        [Test]
        [TestCase("KIN:10.0", "KIN:10.0", ExpectedResult = 0)]
        [TestCase("KIN:10.0", "KIN:100.0", ExpectedResult = -1)]
        [TestCase("KIN:100.0", "KIN:10.0", ExpectedResult = 1)]
        [TestCase("KIN:10.0", "STEEM:10.0", ExpectedResult = -8)]
        [TestCase("STEEM:10.0", "KIN:10.0", ExpectedResult = 8)]
        [TestCase("KIN:10.0", "STEEM:100.0", ExpectedResult = -8)]
        [TestCase("STEEM:100.0", "KIN:10.0", ExpectedResult = 8)]
        public int Test_comparison(string a, string b)
        {
            var aParts = a.Split(':');
            var bParts = b.Split(':');

            var aFee = new Fee(new Asset(aParts[0]), UMoney.Parse(aParts[1]));
            var bFee = new Fee(new Asset(bParts[0]), UMoney.Parse(bParts[1]));

            return aFee.CompareTo(bFee);
        }

        [Test]
        [TestCase("KIN:10.0", "KIN:10.0", ExpectedResult = true)]
        [TestCase("KIN:10.0", "KIN:100.0", ExpectedResult = false)]
        [TestCase("KIN:10.0", "STEEM:10.0", ExpectedResult = false)]
        [TestCase("KIN:10.0", "STEEM:100.0", ExpectedResult = false)]
        public bool Test_hash_code_equation(string a, string b)
        {
            var aParts = a.Split(':');
            var bParts = b.Split(':');

            var aFee = new Fee(new Asset(aParts[0]), UMoney.Parse(aParts[1]));
            var bFee = new Fee(new Asset(bParts[0]), UMoney.Parse(bParts[1]));

            return aFee.GetHashCode() == bFee.GetHashCode();
        }
    }
}
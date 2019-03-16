using System;
using System.Linq;
using Lykke.Bil2.Contract.Common;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class AssetTests
    {
        [Test]
        [TestCase("KIN", "KIN", ExpectedResult = true)]
        [TestCase("KIN:123", "KIN:123", ExpectedResult = true)]
        [TestCase("STEEM", "STEEM", ExpectedResult = true)]
        [TestCase("STEEM:abc", "STEEM:abc", ExpectedResult = true)]
        [TestCase("KIN", "STEEM", ExpectedResult = false)]
        [TestCase("KIN:123", "KIN", ExpectedResult = false)]
        [TestCase("KIN", "KIN:123", ExpectedResult = false)]
        [TestCase("KIN:123", "KIN:126", ExpectedResult = false)]
        [TestCase("KIN:123", "STEEM:123", ExpectedResult = false)]
        public bool Test_equation(string a, string b)
        {
            var aParts = a.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var bParts = b.Split(':', StringSplitOptions.RemoveEmptyEntries);

            var aAsset = new Asset(aParts[0], aParts.Skip(1).SingleOrDefault());
            var bAsset = new Asset(bParts[0], bParts.Skip(1).SingleOrDefault());

            return Equals(aAsset, bAsset);
        }

        [Test]
        [TestCase("KIN", "KIN", ExpectedResult = 0)]
        [TestCase("KIN:123", "KIN:123", ExpectedResult = 0)]
        [TestCase("STEEM", "STEEM", ExpectedResult = 0)]
        [TestCase("STEEM:abc", "STEEM:abc", ExpectedResult = 0)]
        [TestCase("KIN", "STEEM", ExpectedResult = -8)]
        [TestCase("KIN:123", "KIN", ExpectedResult = 1)]
        [TestCase("KIN", "KIN:123", ExpectedResult = -1)]
        [TestCase("KIN:123", "KIN:126", ExpectedResult = -3)]
        [TestCase("KIN:123", "STEEM:123", ExpectedResult = -8)]
        [TestCase("STEEM:abc", "KIN:123", ExpectedResult = 8)]
        public int Test_comparison(string a, string b)
        {
            var aParts = a.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var bParts = b.Split(':', StringSplitOptions.RemoveEmptyEntries);

            var aAsset = new Asset(aParts[0], aParts.Skip(1).SingleOrDefault());
            var bAsset = new Asset(bParts[0], bParts.Skip(1).SingleOrDefault());

            return aAsset.CompareTo(bAsset);
        }

        [Test]
        [TestCase("KIN", "KIN", ExpectedResult = true)]
        [TestCase("KIN:123", "KIN:123", ExpectedResult = true)]
        [TestCase("STEEM", "STEEM", ExpectedResult = true)]
        [TestCase("STEEM:abc", "STEEM:abc", ExpectedResult = true)]
        [TestCase("KIN", "STEEM", ExpectedResult = false)]
        [TestCase("KIN:123", "KIN", ExpectedResult = false)]
        [TestCase("KIN", "KIN:123", ExpectedResult = false)]
        [TestCase("KIN:123", "KIN:126", ExpectedResult = false)]
        [TestCase("KIN:123", "STEEM:123", ExpectedResult = false)]
        public bool Test_hash_code_equation(string a, string b)
        {
            var aParts = a.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var bParts = b.Split(':', StringSplitOptions.RemoveEmptyEntries);

            var aAsset = new Asset(aParts[0], aParts.Skip(1).SingleOrDefault());
            var bAsset = new Asset(bParts[0], bParts.Skip(1).SingleOrDefault());

            return aAsset.GetHashCode() == bAsset.GetHashCode();
        }
    }
}

using Lykke.Bil2.Contract.BlocksReader;
using Lykke.Bil2.Contract.Common;
using NUnit.Framework;

namespace Lykke.Bil2.Contract.Tests
{
    [TestFixture]
    public class ImplicitToStringValueTypesTests
    {
        private class TestImplicitToStringType : BaseImplicitToStringValueType<TestImplicitToStringType>
        {
            public TestImplicitToStringType(string value) :
                base(value)
            {
            }
        }

        [Test]
        [TestCase("123", ExpectedResult = "123")]
        [TestCase(null, ExpectedResult = null)]
        public string TestAssetIdConversion(string value) => (AssetId) value;

        [Test]
        [TestCase("123", ExpectedResult = "123")]
        [TestCase(null, ExpectedResult = null)]
        public string TestAddressConversion(string value) => (Address) value;

        [Test]
        [TestCase("123", ExpectedResult = "123")]
        [TestCase(null, ExpectedResult = null)]
        public string TestAddressTagConversion(string value) => (AddressTag) value;

        [Test]
        [TestCase("123", ExpectedResult = "123")]
        [TestCase(null, ExpectedResult = null)]
        public string TestBalanceChangeIdConversion(string value) => (BalanceChangeId) value;

        [Test]
        [TestCase("123", "1234", ExpectedResult = -52)]
        [TestCase("1234", "123", ExpectedResult = 52)]
        [TestCase("123", "123", ExpectedResult = 0)]
        public int TestComparison(string a, string b) => 
            new TestImplicitToStringType(a).CompareTo(new TestImplicitToStringType(b));
       
        [Test]
        [TestCase("123", "1234", ExpectedResult = false)]
        [TestCase("1234", "123", ExpectedResult = false)]
        [TestCase("123", "123", ExpectedResult = true)]
        public bool TestEquation(string a, string b) => 
            new TestImplicitToStringType(a).Equals(new TestImplicitToStringType(b));

        [Test]
        [TestCase("123", "1234", ExpectedResult = false)]
        [TestCase("1234", "123", ExpectedResult = false)]
        [TestCase("123", "123", ExpectedResult = true)]
        public bool TestEquationToObject(string a, string b) => 
            new TestImplicitToStringType(a).Equals((object)new TestImplicitToStringType(b));

        [Test]
        [TestCase("123", "1234", ExpectedResult = false)]
        [TestCase("1234", "123", ExpectedResult = false)]
        [TestCase("123", "123", ExpectedResult = true)]
        public bool TestHashCode(string a, string b) => 
            new TestImplicitToStringType(a).GetHashCode() == new TestImplicitToStringType(b).GetHashCode();
    }
}

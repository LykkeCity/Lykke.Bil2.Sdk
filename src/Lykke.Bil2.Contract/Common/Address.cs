using JetBrains.Annotations;

namespace Lykke.Bil2.Contract.Common
{
    [PublicAPI]
    public sealed class Address : BaseImplicitToStringValueType<Address>
    {
        public Address(string value) :
            base(value)
        {
        }

        public static implicit operator Address(string value)
        {
            return value != null ? new Address(value) : null;
        }
   
        public static implicit operator string(Address value)
        {
            return value?.Value;
        }
    }
}

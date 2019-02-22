using JetBrains.Annotations;

namespace Lykke.Bil2.Contract.Common
{
    [PublicAPI]
    public sealed class AddressTag : BaseImplicitToStringValueType<AddressTag>
    {
        public AddressTag(string value) :
            base(value)
        {
        }

        public static implicit operator AddressTag(string value)
        {
            return value != null ? new AddressTag(value) : null;
        }
   
        public static implicit operator string(AddressTag value)
        {
            return value?.Value;
        }
    }
}

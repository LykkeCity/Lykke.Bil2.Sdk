using JetBrains.Annotations;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Contract.Common
{
    [PublicAPI]
    
    public sealed class Semver : BaseImplicitToStringValueType<Semver>
    {
        public Semver(string value) :
            base(value)
        {
        }

        public static implicit operator Semver(string value)
        {
            return value != null ? new Semver(value) : null;
        }
   
        public static implicit operator string(Semver value)
        {
            return value?.Value;
        }
    }
}

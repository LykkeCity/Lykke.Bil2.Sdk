using JetBrains.Annotations;

namespace Lykke.Bil2.Contract.Common
{
    [PublicAPI]
    
    public sealed class AssetAddress : BaseImplicitToStringValueType<AssetAddress>
    {
        public AssetAddress(string value) :
            base(value)
        {
        }

        public static implicit operator AssetAddress(string value)
        {
            return value != null ? new AssetAddress(value) : null;
        }
   
        public static implicit operator string(AssetAddress value)
        {
            return value?.Value;
        }
    }
}
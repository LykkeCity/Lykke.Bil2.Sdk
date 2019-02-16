using JetBrains.Annotations;

namespace Lykke.Blockchains.Integrations.Contract.Common
{
    [PublicAPI]
    
    public sealed class AssetId : BaseImplicitToStringValueType<AssetId>
    {
        public AssetId(string value) :
            base(value)
        {
        }

        public static implicit operator AssetId(string value)
        {
            return value != null ? new AssetId(value) : null;
        }
   
        public static implicit operator string(AssetId value)
        {
            return value?.Value;
        }
    }
}

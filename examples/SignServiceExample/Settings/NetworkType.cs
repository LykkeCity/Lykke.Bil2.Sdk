using System.Runtime.Serialization;

namespace SignServiceExample.Settings
{
    public enum NetworkType
    {
        [EnumMember(Value = "test")]
        Test,

        [EnumMember(Value = "main")]
        Main
    }
}

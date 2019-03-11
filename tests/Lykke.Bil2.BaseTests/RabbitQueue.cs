using System.Runtime.Serialization;

namespace Lykke.Bil2.BaseTests
{
    [DataContract]
    public class RabbitQueue
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}

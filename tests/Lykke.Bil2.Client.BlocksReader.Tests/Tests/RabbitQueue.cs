using System.Runtime.Serialization;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Tests
{
    [DataContract]
    public class RabbitQueue
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
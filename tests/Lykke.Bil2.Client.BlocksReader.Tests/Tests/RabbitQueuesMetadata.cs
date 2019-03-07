using System.Runtime.Serialization;

namespace Lykke.Bil2.Client.BlocksReader.Tests.Tests
{
    [DataContract]
    public class RabbitQueuesMetadata
    {
        [DataMember(Name = "items")]
        public RabbitQueue[] Items { get; set; }
    }
}
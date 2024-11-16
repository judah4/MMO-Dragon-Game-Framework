using System.Runtime.Serialization;
namespace Mmogf.Servers.Contracts
{
    [DataContract]
    public struct EventRequest
    {
        [DataMember(Order = 0)]
        public EntityId EntityId { get; set; }
        [DataMember(Order = 1)]
        public short ComponentId { get; set; }
        [DataMember(Order = 2)]
        public short EventId { get; set; }
        [DataMember(Order = 3)]
        public byte[] Payload { get; set; }
    }
}
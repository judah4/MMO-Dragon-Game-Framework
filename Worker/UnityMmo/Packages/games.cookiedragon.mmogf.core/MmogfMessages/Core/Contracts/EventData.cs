using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct EventData
    {
        [DataMember(Order = 1)]
        public int EntityId { get; set; }
        [DataMember(Order = 2)]
        public short ComponentId { get; set; }
        [DataMember(Order = 3)]
        public byte[] Payload { get; set; }
    }
}
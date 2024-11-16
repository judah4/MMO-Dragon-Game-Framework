using System.Runtime.Serialization;
namespace Mmogf.Servers.Contracts
{
    [DataContract]
    public struct CommandRequest
    {
        [DataMember(Order = 0)]
        public string RequestId { get; set; }

        [DataMember(Order = 2)]
        public long RequesterId { get; set; }

        [DataMember(Order = 3)]
        public string RequestorWorkerType { get; set; }

        [DataMember(Order = 3)]
        public EntityId EntityId { get; set; }
        [DataMember(Order = 4)]
        public short ComponentId { get; set; }
        [DataMember(Order = 5)]
        public short CommandId { get; set; }
        [DataMember(Order = 6)]
        public byte[] Payload { get; set; }
    }
}
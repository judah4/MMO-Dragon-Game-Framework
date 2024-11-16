using System.Runtime.Serialization;
namespace Mmogf.Servers.Contracts
{
    [DataContract]
    public struct EntityUpdate
    {
        [DataMember(Order = 0)]
        public EntityId EntityId { get; set; }
        [DataMember(Order = 1)]
        public short ComponentId { get; set; }
        [DataMember(Order = 2)]
        public byte[] Info { get; set; }
    }
}
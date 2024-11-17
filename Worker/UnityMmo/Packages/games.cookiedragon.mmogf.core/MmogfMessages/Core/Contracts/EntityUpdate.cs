using Mmogf.Servers.Shared;
using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
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
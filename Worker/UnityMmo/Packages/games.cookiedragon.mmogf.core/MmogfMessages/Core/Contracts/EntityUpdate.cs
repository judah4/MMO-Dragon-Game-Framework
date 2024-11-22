using Mmogf.Servers.Shared;
using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct EntityUpdate
    {
        [DataMember(Order = 1)]
        public EntityId EntityId { get; set; }
        [DataMember(Order = 2)]
        public short ComponentId { get; set; }
        [DataMember(Order = 3)]
        public byte[] Info { get; set; }
    }
}
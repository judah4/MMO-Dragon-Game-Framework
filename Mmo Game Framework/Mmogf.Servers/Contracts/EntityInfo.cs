using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Mmogf.Servers.Contracts
{
    [DataContract]
    public struct EntityInfo
    {
        [DataMember(Order = 0)]
        public EntityId EntityId { get; set; }
        [DataMember(Order = 1)]
        public Dictionary<short, byte[]> EntityData { get; set; }

    }
}
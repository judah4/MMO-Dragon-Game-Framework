using Mmogf.Servers.Shared;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct EntityInfo
    {
        [DataMember(Order = 1)]
        public EntityId EntityId { get; set; }
        [DataMember(Order = 2)]
        public Dictionary<short, byte[]> EntityData { get; set; }

    }
}
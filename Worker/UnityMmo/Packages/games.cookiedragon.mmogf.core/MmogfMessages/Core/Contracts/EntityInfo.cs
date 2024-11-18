using Mmogf.Servers.Shared;
using System.Collections.Generic;
using MessagePack;
namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct EntityInfo
    {
        [Key(0)]
        public EntityId EntityId { get; set; }
        [Key(1)]
        public Dictionary<short, byte[]> EntityData { get; set; }

    }
}
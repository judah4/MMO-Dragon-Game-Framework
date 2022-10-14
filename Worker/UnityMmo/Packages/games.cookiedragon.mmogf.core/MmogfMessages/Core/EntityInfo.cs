using MessagePack;
using Mmogf.Core;
using System.Collections.Generic;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct EntityInfo : IMessage
    {
        [Key(0)]
        public int EntityId { get; set; }
        [Key(1)]
        public Dictionary<short, byte[]> EntityData { get; set; }

    }
}
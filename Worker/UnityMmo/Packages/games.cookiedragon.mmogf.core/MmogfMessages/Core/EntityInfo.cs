using MessagePack;
using Mmogf.Core;
using System.Collections.Generic;
namespace Mmogf.Core
{
    [MessagePackObject]
    public partial struct EntityInfo : IMessage
    {
        [Key(0)]
        public int EntityId { get; set; }
        [Key(1)]
        public Dictionary<int, byte[]> EntityData { get; set; }

    }
}
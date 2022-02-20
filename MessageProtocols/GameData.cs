using MessagePack;
using Mmogf.Core;
using System.Collections.Generic;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct GameData
    {
        [Key(0)]
        public int EntityId { get; set; }
        [Key(1)]
        public byte[] Info { get; set; }

    }
}
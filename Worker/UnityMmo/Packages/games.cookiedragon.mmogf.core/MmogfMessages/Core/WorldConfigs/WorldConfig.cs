using MessagePack;
using Mmogf.Core;
using System.Collections.Generic;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct WorldConfig : IMessage
    {
        [Key(0)]
        public string Version { get; set; }

        [Key(1)]
        public List<EntityWorldConfig> Entities { get; set; }


    }
}
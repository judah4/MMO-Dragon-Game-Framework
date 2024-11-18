using System.Collections.Generic;
using MessagePack;
namespace Mmogf.Core.Contracts
{
    [MessagePackObject]
    public struct WorldConfig
    {
        [Key(0)]
        public string Version { get; set; }

        [Key(1)]
        public List<EntityWorldConfig> Entities { get; set; }


    }
}
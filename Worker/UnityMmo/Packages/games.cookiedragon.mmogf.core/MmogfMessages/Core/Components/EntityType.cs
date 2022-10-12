
using MessagePack;

namespace Mmogf.Core 
{

    [MessagePackObject]
    public struct EntityType : IEntityComponent
    {
        public static short ComponentId = 1;
        public short GetComponentId() => ComponentId;

        [Key(0)]
        public string Name { get; set; }

    }

}


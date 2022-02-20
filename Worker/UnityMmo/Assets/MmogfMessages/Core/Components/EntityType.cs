
using MessagePack;

namespace Mmogf.Core 
{

    [MessagePackObject]
    public struct EntityType : IEntityComponent
    {
        public static int ComponentId = 1;
        public int GetComponentId() => ComponentId;

        [Key(0)]
        public string Name { get; set; }

    }

}


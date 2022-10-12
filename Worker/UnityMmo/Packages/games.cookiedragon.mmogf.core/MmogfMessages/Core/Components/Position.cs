using MessagePack;
using Mmogf.Core;

namespace Mmogf.Core
{
    [MessagePackObject]
    public partial struct Position : IEntityComponent
    {
        public static short ComponentId = 2;
        public short GetComponentId() => ComponentId;

        [Key(0)]
        public double X { get; set; }
        [Key(1)]
        public double Y { get; set; }
        [Key(2)]
        public double Z { get; set; }

    }
}
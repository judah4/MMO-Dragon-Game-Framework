using MessagePack;
using Mmogf.Core;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct Rotation : IEntityComponent
    {
        public static int ComponentId = 3;
        public int GetComponentId() => ComponentId;

        [Key(0)]
        public float X { get; set; }
        [Key(1)]
        public float Y { get; set; }
        [Key(2)]
        public float Z { get; set; }
        [Key(3)]
        public float W { get; set; }
    }
}
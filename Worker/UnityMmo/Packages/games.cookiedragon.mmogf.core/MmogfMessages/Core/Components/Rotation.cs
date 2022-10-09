using MessagePack;
using Mmogf.Core;
namespace Mmogf.Core
{
    [MessagePackObject]
    public struct Rotation : IEntityComponent
    {

        public static Rotation Zero => new Rotation()
        {
            Heading = 0,
        };

        public static int ComponentId = 3;
        public int GetComponentId() => ComponentId;

        [Key(0)]
        public float Heading { get; set; }
    }
}
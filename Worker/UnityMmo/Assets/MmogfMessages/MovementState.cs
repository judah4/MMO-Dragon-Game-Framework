using MessagePack;
using Mmogf.Core.Contracts;

namespace Mmogf
{
    [MessagePackObject]
    public struct MovementState : IEntityComponent
    {
        public const short ComponentId = 1004;
        public short GetComponentId()
        {
            return ComponentId;
        }

        [Key(0)]
        public float Forward { get; set; }
        [Key(1)]
        public float Heading { get; set; }

        [Key(2)]
        public Mmogf.Vector3d DesiredPosition { get; set; }

    }
}

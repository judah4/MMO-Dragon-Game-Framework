using MessagePack;

namespace Mmogf.Core
{

    /// <summary>
    /// 21.10 Fixed point decimal for entity position.
    /// </summary>
    [MessagePackObject]
    public partial struct FixedVector3 : IEntityComponent
    {
        public const short ComponentId = 2;
        public short GetComponentId() => ComponentId;

        [Key(0)]
        public int X { get; set; }
        [Key(1)]
        public int Y { get; set; }
        [Key(2)]
        public int Z { get; set; }

        public Position ToPosition()
        {
            return Position.FromFixedVector3(this);
        }

    }
}
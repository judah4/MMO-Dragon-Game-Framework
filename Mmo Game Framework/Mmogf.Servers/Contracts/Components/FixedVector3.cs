using System.Runtime.Serialization;

namespace Mmogf.Servers.Contracts
{

    /// <summary>
    /// 21.10 Fixed point decimal for entity position.
    /// </summary>
    [DataContract]
    public partial struct FixedVector3 : IEntityComponent
    {
        public const short ComponentId = 2;
        public short GetComponentId() => ComponentId;

        [DataMember(Order = 0)]
        public int X { get; set; }
        [DataMember(Order = 1)]
        public int Y { get; set; }
        [DataMember(Order = 2)]
        public int Z { get; set; }

        public Position ToPosition()
        {
            return Position.FromFixedVector3(this);
        }

    }
}
using Mmogf.Core.Contracts;
using System.Runtime.Serialization;

namespace Mmogf
{
    [DataContract]
    public struct MovementState : IEntityComponent
    {
        public const short ComponentId = 1004;
        public short GetComponentId()
        {
            return ComponentId;
        }

        [DataMember(Order = 1)]
        public float Forward { get; set; }
        [DataMember(Order = 2)]
        public float Heading { get; set; }

        [DataMember(Order = 3)]
        public Mmogf.Vector3d DesiredPosition { get; set; }

    }
}

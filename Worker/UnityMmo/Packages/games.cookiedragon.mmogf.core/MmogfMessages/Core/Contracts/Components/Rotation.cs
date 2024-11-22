using System.Runtime.Serialization;
namespace Mmogf.Core.Contracts
{
    [DataContract]
    public struct Rotation : IEntityComponent
    {

        public static Rotation Zero => new Rotation()
        {
            Heading = 0,
        };

        public const short ComponentId = 3;
        public short GetComponentId() => ComponentId;

        [DataMember(Order = 1)]
        public short Heading { get; set; }
    }
}
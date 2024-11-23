using System.Runtime.Serialization;

namespace Mmogf.Core.Contracts
{

    [DataContract]
    public struct EntityType : IEntityComponent
    {
        public const short ComponentId = 1;
        public short GetComponentId() => ComponentId;

        [DataMember(Order = 1)]
        public string Name { get; set; }
    }
}

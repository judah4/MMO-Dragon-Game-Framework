using System.Runtime.Serialization;

namespace Mmogf.Servers.Contracts
{

    [DataContract]
    public struct EntityType : IEntityComponent
    {
        public const short ComponentId = 1;
        public short GetComponentId() => ComponentId;

        [DataMember(Order = 0)]
        public string Name { get; set; }

    }

}

